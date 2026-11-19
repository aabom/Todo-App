import type { CreateTodoInput, Priority, Todo, TodoApi, TodoStatistics, UpdateTodoInput } from '../api/types'

const RANK: Record<Priority, number> = { CRITICAL: 0, HIGH: 1, MEDIUM: 2, LOW: 3 }

let nextId = 1

export function makeTodo(overrides: Partial<Todo> & Pick<Todo, 'title'>): Todo {
  return {
    id: `todo-${String(nextId++)}`,
    description: null,
    isCompleted: false,
    priority: 'MEDIUM',
    createdAt: '2026-01-15T12:00:00.000Z',
    completedAt: null,
    dueDate: null,
    isOverdue: false,
    ...overrides,
  }
}

/**
 * Stands in for the GraphQL API. It sorts the way the server does (priority, then due
 * date with undated last, then newest first) and records what the UI sends.
 */
export class FakeTodoApi implements TodoApi {
  private items: Todo[] = []

  readonly updates: UpdateTodoInput[] = []
  readonly creates: CreateTodoInput[] = []

  /** When set, fetching statistics fails, as it would if the API went away mid-session. */
  statisticsFail = false

  /** When set, creating a todo fails with this error. */
  createError: Error | null = null

  seed(...todos: Todo[]): this {
    this.items.push(...todos)
    return this
  }

  getTodos(): Promise<Todo[]> {
    const sorted = [...this.items].sort(
      (a, b) =>
        RANK[a.priority] - RANK[b.priority] ||
        Number(a.dueDate === null) - Number(b.dueDate === null) ||
        (a.dueDate ?? '').localeCompare(b.dueDate ?? '') ||
        b.createdAt.localeCompare(a.createdAt),
    )
    return Promise.resolve(sorted)
  }

  getStatistics(): Promise<TodoStatistics> {
    if (this.statisticsFail) return Promise.reject(new Error('API unreachable'))
    const completed = this.items.filter((t) => t.isCompleted).length
    return Promise.resolve({
      totalCount: this.items.length,
      completedCount: completed,
      pendingCount: this.items.length - completed,
      overdueCount: this.items.filter((t) => t.isOverdue).length,
    })
  }

  createTodo(input: CreateTodoInput): Promise<Todo> {
    this.creates.push(input)
    if (this.createError) return Promise.reject(this.createError)
    const todo = makeTodo({
      title: input.title.trim(),
      description: input.description ?? null,
      priority: input.priority ?? 'MEDIUM',
      dueDate: input.dueDate ?? null,
      createdAt: new Date().toISOString(),
    })
    this.items.push(todo)
    return Promise.resolve(todo)
  }

  updateTodo(input: UpdateTodoInput): Promise<Todo | null> {
    this.updates.push(input)
    const todo = this.items.find((t) => t.id === input.id)
    if (!todo) return Promise.resolve(null)
    if (input.title !== undefined) todo.title = input.title
    if (input.description !== undefined) todo.description = input.description || null
    if (input.priority !== undefined) todo.priority = input.priority
    if (input.clearDueDate) todo.dueDate = null
    else if (input.dueDate !== undefined) todo.dueDate = input.dueDate
    return Promise.resolve({ ...todo })
  }

  toggleTodo(id: string): Promise<Todo | null> {
    const todo = this.items.find((t) => t.id === id)
    if (!todo) return Promise.resolve(null)
    todo.isCompleted = !todo.isCompleted
    todo.completedAt = todo.isCompleted ? new Date().toISOString() : null
    return Promise.resolve({ ...todo })
  }

  deleteTodo(id: string): Promise<boolean> {
    const before = this.items.length
    this.items = this.items.filter((t) => t.id !== id)
    return Promise.resolve(this.items.length < before)
  }
}

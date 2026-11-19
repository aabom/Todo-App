// Shapes of the GraphQL schema served by TodoApp.Api. Dates travel as ISO 8601 strings
// in UTC (with a trailing "Z"); conversion to local time happens only for display.

export const PRIORITIES = ['LOW', 'MEDIUM', 'HIGH', 'CRITICAL'] as const

export type Priority = (typeof PRIORITIES)[number]

export interface Todo {
  id: string
  title: string
  description: string | null
  isCompleted: boolean
  priority: Priority
  createdAt: string
  completedAt: string | null
  dueDate: string | null
  isOverdue: boolean
}

export interface TodoStatistics {
  totalCount: number
  completedCount: number
  pendingCount: number
  overdueCount: number
}

export interface CreateTodoInput {
  title: string
  description?: string | null
  priority?: Priority
  dueDate?: string | null
}

/**
 * A partial update, mirroring the API: a field left out is unchanged, an empty
 * description removes it, and `clearDueDate` removes the due date.
 */
export interface UpdateTodoInput {
  id: string
  title?: string
  description?: string
  priority?: Priority
  dueDate?: string
  clearDueDate?: boolean
}

/** Everything the UI needs from the server. Tests substitute their own implementation. */
export interface TodoApi {
  getTodos(): Promise<Todo[]>
  getStatistics(): Promise<TodoStatistics>
  createTodo(input: CreateTodoInput): Promise<Todo>
  updateTodo(input: UpdateTodoInput): Promise<Todo | null>
  toggleTodo(id: string): Promise<Todo | null>
  deleteTodo(id: string): Promise<boolean>
}

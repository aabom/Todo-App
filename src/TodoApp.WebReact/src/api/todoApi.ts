import type { GraphQLExecutor } from './graphqlClient'
import type { CreateTodoInput, Todo, TodoApi, TodoStatistics, UpdateTodoInput } from './types'

const TODO_FIELDS = `
  id
  title
  description
  isCompleted
  priority
  createdAt
  completedAt
  dueDate
  isOverdue
`

const GET_TODOS = `query GetTodos { todos { ${TODO_FIELDS} } }`

const GET_STATISTICS = `
  query GetStatistics {
    statistics { totalCount completedCount pendingCount overdueCount }
  }
`

const CREATE_TODO = `
  mutation CreateTodo($input: CreateTodoInput!) {
    createTodo(input: $input) { ${TODO_FIELDS} }
  }
`

const UPDATE_TODO = `
  mutation UpdateTodo($input: UpdateTodoInput!) {
    updateTodo(input: $input) { ${TODO_FIELDS} }
  }
`

const TOGGLE_TODO = `
  mutation ToggleTodo($id: UUID!) {
    toggleTodoComplete(id: $id) { ${TODO_FIELDS} }
  }
`

const DELETE_TODO = `
  mutation DeleteTodo($id: UUID!) {
    deleteTodo(id: $id) { success }
  }
`

/** The API's operations, each typed by the shape of its `data`. */
export function createTodoApi(execute: GraphQLExecutor): TodoApi {
  return {
    async getTodos() {
      const data = await execute<{ todos: Todo[] }>(GET_TODOS)
      return data.todos
    },
    async getStatistics() {
      const data = await execute<{ statistics: TodoStatistics }>(GET_STATISTICS)
      return data.statistics
    },
    async createTodo(input: CreateTodoInput) {
      const data = await execute<{ createTodo: Todo }>(CREATE_TODO, { input })
      return data.createTodo
    },
    async updateTodo(input: UpdateTodoInput) {
      const data = await execute<{ updateTodo: Todo | null }>(UPDATE_TODO, { input })
      return data.updateTodo
    },
    async toggleTodo(id: string) {
      const data = await execute<{ toggleTodoComplete: Todo | null }>(TOGGLE_TODO, { id })
      return data.toggleTodoComplete
    },
    async deleteTodo(id: string) {
      const data = await execute<{ deleteTodo: { success: boolean } }>(DELETE_TODO, { id })
      return data.deleteTodo.success
    },
  }
}

// The app's view of the API. What the server sends back is generated from its schema
// (src/gql, `npm run codegen`). Only the inputs are written by hand, because they carry the
// partial-update rules below; the generated variable types check them at every call site.
// Dates travel as ISO 8601 strings in UTC (with a trailing "Z"); conversion to local time
// happens only for display.

import type { GetStatisticsQuery, Priority, TodoFieldsFragment } from '../gql/graphql'

export type { Priority }

/** In the order the UI offers them, lowest first. */
export const PRIORITIES = ['LOW', 'MEDIUM', 'HIGH', 'CRITICAL'] as const satisfies readonly Priority[]

// Fails to compile if the API gains a priority that the list above does not offer.
type Assert<T extends true> = T
export type AllPrioritiesListed = Assert<Exclude<Priority, (typeof PRIORITIES)[number]> extends never ? true : false>

export type Todo = TodoFieldsFragment

export type TodoStatistics = GetStatisticsQuery['statistics']

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

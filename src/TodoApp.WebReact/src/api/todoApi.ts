import { graphql } from '../gql'
import type { GraphQLExecutor } from './graphqlClient'
import type { CreateTodoInput, TodoApi, UpdateTodoInput } from './types'

// The operations the client sends. `npm run codegen` reads these and generates their result
// and variable types into src/gql from the API's schema, so a field the server does not have,
// or a variable of the wrong type, fails the type check rather than the request.

graphql(`
  fragment TodoFields on TodoItem {
    id
    title
    description
    isCompleted
    priority
    createdAt
    completedAt
    dueDate
    isOverdue
  }
`)

const GetTodos = graphql(`
  query GetTodos {
    todos { ...TodoFields }
  }
`)

const GetStatistics = graphql(`
  query GetStatistics {
    statistics { totalCount completedCount pendingCount overdueCount }
  }
`)

const CreateTodo = graphql(`
  mutation CreateTodo($input: CreateTodoInput!) {
    createTodo(input: $input) { ...TodoFields }
  }
`)

const UpdateTodo = graphql(`
  mutation UpdateTodo($input: UpdateTodoInput!) {
    updateTodo(input: $input) { ...TodoFields }
  }
`)

const ToggleTodo = graphql(`
  mutation ToggleTodo($id: UUID!) {
    toggleTodoComplete(id: $id) { ...TodoFields }
  }
`)

const DeleteTodo = graphql(`
  mutation DeleteTodo($id: UUID!) {
    deleteTodo(id: $id) { success }
  }
`)

/** The API's operations, each typed by its generated document. */
export function createTodoApi(execute: GraphQLExecutor): TodoApi {
  return {
    async getTodos() {
      return (await execute(GetTodos)).todos
    },
    async getStatistics() {
      return (await execute(GetStatistics)).statistics
    },
    async createTodo(input: CreateTodoInput) {
      return (await execute(CreateTodo, { input })).createTodo
    },
    async updateTodo(input: UpdateTodoInput) {
      return (await execute(UpdateTodo, { input })).updateTodo ?? null
    },
    async toggleTodo(id: string) {
      return (await execute(ToggleTodo, { id })).toggleTodoComplete ?? null
    },
    async deleteTodo(id: string) {
      return (await execute(DeleteTodo, { id })).deleteTodo.success
    },
  }
}

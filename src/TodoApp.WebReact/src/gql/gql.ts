/* eslint-disable */
import * as types from './graphql';



/**
 * Map of all GraphQL operations in the project.
 *
 * This map has several performance disadvantages:
 * 1. It is not tree-shakeable, so it will include all operations in the project.
 * 2. It is not minifiable, so the string of a GraphQL query will be multiple times inside the bundle.
 * 3. It does not support dead code elimination, so it will add unused operations.
 *
 * Therefore it is highly recommended to use the babel or swc plugin for production.
 * Learn more about it here: https://the-guild.dev/graphql/codegen/plugins/presets/preset-client#reducing-bundle-size
 */
type Documents = {
    "\n  fragment TodoFields on TodoItem {\n    id\n    title\n    description\n    isCompleted\n    priority\n    createdAt\n    completedAt\n    dueDate\n    isOverdue\n  }\n": typeof types.TodoFieldsFragmentDoc,
    "\n  query GetTodos {\n    todos { ...TodoFields }\n  }\n": typeof types.GetTodosDocument,
    "\n  query GetStatistics {\n    statistics { totalCount completedCount pendingCount overdueCount }\n  }\n": typeof types.GetStatisticsDocument,
    "\n  mutation CreateTodo($input: CreateTodoInput!) {\n    createTodo(input: $input) { ...TodoFields }\n  }\n": typeof types.CreateTodoDocument,
    "\n  mutation UpdateTodo($input: UpdateTodoInput!) {\n    updateTodo(input: $input) { ...TodoFields }\n  }\n": typeof types.UpdateTodoDocument,
    "\n  mutation ToggleTodo($id: UUID!) {\n    toggleTodoComplete(id: $id) { ...TodoFields }\n  }\n": typeof types.ToggleTodoDocument,
    "\n  mutation DeleteTodo($id: UUID!) {\n    deleteTodo(id: $id) { success }\n  }\n": typeof types.DeleteTodoDocument,
};
const documents: Documents = {
    "\n  fragment TodoFields on TodoItem {\n    id\n    title\n    description\n    isCompleted\n    priority\n    createdAt\n    completedAt\n    dueDate\n    isOverdue\n  }\n": types.TodoFieldsFragmentDoc,
    "\n  query GetTodos {\n    todos { ...TodoFields }\n  }\n": types.GetTodosDocument,
    "\n  query GetStatistics {\n    statistics { totalCount completedCount pendingCount overdueCount }\n  }\n": types.GetStatisticsDocument,
    "\n  mutation CreateTodo($input: CreateTodoInput!) {\n    createTodo(input: $input) { ...TodoFields }\n  }\n": types.CreateTodoDocument,
    "\n  mutation UpdateTodo($input: UpdateTodoInput!) {\n    updateTodo(input: $input) { ...TodoFields }\n  }\n": types.UpdateTodoDocument,
    "\n  mutation ToggleTodo($id: UUID!) {\n    toggleTodoComplete(id: $id) { ...TodoFields }\n  }\n": types.ToggleTodoDocument,
    "\n  mutation DeleteTodo($id: UUID!) {\n    deleteTodo(id: $id) { success }\n  }\n": types.DeleteTodoDocument,
};

/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "\n  fragment TodoFields on TodoItem {\n    id\n    title\n    description\n    isCompleted\n    priority\n    createdAt\n    completedAt\n    dueDate\n    isOverdue\n  }\n"): typeof import('./graphql').TodoFieldsFragmentDoc;
/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "\n  query GetTodos {\n    todos { ...TodoFields }\n  }\n"): typeof import('./graphql').GetTodosDocument;
/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "\n  query GetStatistics {\n    statistics { totalCount completedCount pendingCount overdueCount }\n  }\n"): typeof import('./graphql').GetStatisticsDocument;
/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "\n  mutation CreateTodo($input: CreateTodoInput!) {\n    createTodo(input: $input) { ...TodoFields }\n  }\n"): typeof import('./graphql').CreateTodoDocument;
/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "\n  mutation UpdateTodo($input: UpdateTodoInput!) {\n    updateTodo(input: $input) { ...TodoFields }\n  }\n"): typeof import('./graphql').UpdateTodoDocument;
/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "\n  mutation ToggleTodo($id: UUID!) {\n    toggleTodoComplete(id: $id) { ...TodoFields }\n  }\n"): typeof import('./graphql').ToggleTodoDocument;
/**
 * The graphql function is used to parse GraphQL queries into a document that can be used by GraphQL clients.
 */
export function graphql(source: "\n  mutation DeleteTodo($id: UUID!) {\n    deleteTodo(id: $id) { success }\n  }\n"): typeof import('./graphql').DeleteTodoDocument;


export function graphql(source: string) {
  return (documents as any)[source] ?? {};
}

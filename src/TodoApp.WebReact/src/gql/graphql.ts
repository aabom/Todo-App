/* eslint-disable */
/** Internal type. DO NOT USE DIRECTLY. */
type Exact<T extends { [key: string]: unknown }> = { [K in keyof T]: T[K] };
/** Internal type. DO NOT USE DIRECTLY. */
export type Incremental<T> = T | { [P in keyof T]?: P extends ' $fragmentName' | '__typename' ? T[P] : never };
import type { DocumentTypeDecoration } from '@graphql-typed-document-node/core';
export type CreateTodoInput = {
  description?: string | null | undefined;
  dueDate?: string | null | undefined;
  priority?: Priority;
  title: string;
};

export type Priority =
  | 'CRITICAL'
  | 'HIGH'
  | 'LOW'
  | 'MEDIUM';

export type UpdateTodoInput = {
  clearDueDate?: boolean;
  description?: string | null | undefined;
  dueDate?: string | null | undefined;
  id: string;
  priority?: Priority | null | undefined;
  title?: string | null | undefined;
};

export type TodoFieldsFragment = { id: string, title: string, description: string | null, isCompleted: boolean, priority: Priority, createdAt: string, completedAt: string | null, dueDate: string | null, isOverdue: boolean };

export type GetTodosQueryVariables = Exact<{ [key: string]: never; }>;


export type GetTodosQuery = { todos: Array<{ id: string, title: string, description: string | null, isCompleted: boolean, priority: Priority, createdAt: string, completedAt: string | null, dueDate: string | null, isOverdue: boolean }> };

export type GetStatisticsQueryVariables = Exact<{ [key: string]: never; }>;


export type GetStatisticsQuery = { statistics: { totalCount: number, completedCount: number, pendingCount: number, overdueCount: number } };

export type CreateTodoMutationVariables = Exact<{
  input: CreateTodoInput;
}>;


export type CreateTodoMutation = { createTodo: { id: string, title: string, description: string | null, isCompleted: boolean, priority: Priority, createdAt: string, completedAt: string | null, dueDate: string | null, isOverdue: boolean } };

export type UpdateTodoMutationVariables = Exact<{
  input: UpdateTodoInput;
}>;


export type UpdateTodoMutation = { updateTodo: { id: string, title: string, description: string | null, isCompleted: boolean, priority: Priority, createdAt: string, completedAt: string | null, dueDate: string | null, isOverdue: boolean } | null };

export type ToggleTodoMutationVariables = Exact<{
  id: string;
}>;


export type ToggleTodoMutation = { toggleTodoComplete: { id: string, title: string, description: string | null, isCompleted: boolean, priority: Priority, createdAt: string, completedAt: string | null, dueDate: string | null, isOverdue: boolean } | null };

export type DeleteTodoMutationVariables = Exact<{
  id: string;
}>;


export type DeleteTodoMutation = { deleteTodo: { success: boolean } };

export class TypedDocumentString<TResult, TVariables>
  extends String
  implements DocumentTypeDecoration<TResult, TVariables>
{
  __apiType?: NonNullable<DocumentTypeDecoration<TResult, TVariables>['__apiType']>;
  private value: string;
  public __meta__?: Record<string, any> | undefined;

  constructor(value: string, __meta__?: Record<string, any> | undefined) {
    super(value);
    this.value = value;
    this.__meta__ = __meta__;
  }

  override toString(): string & DocumentTypeDecoration<TResult, TVariables> {
    return this.value;
  }
}
export const TodoFieldsFragmentDoc = new TypedDocumentString(`
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
    `, {"fragmentName":"TodoFields"}) as unknown as TypedDocumentString<TodoFieldsFragment, unknown>;
export const GetTodosDocument = new TypedDocumentString(`
    query GetTodos {
  todos {
    ...TodoFields
  }
}
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
}`) as unknown as TypedDocumentString<GetTodosQuery, GetTodosQueryVariables>;
export const GetStatisticsDocument = new TypedDocumentString(`
    query GetStatistics {
  statistics {
    totalCount
    completedCount
    pendingCount
    overdueCount
  }
}
    `) as unknown as TypedDocumentString<GetStatisticsQuery, GetStatisticsQueryVariables>;
export const CreateTodoDocument = new TypedDocumentString(`
    mutation CreateTodo($input: CreateTodoInput!) {
  createTodo(input: $input) {
    ...TodoFields
  }
}
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
}`) as unknown as TypedDocumentString<CreateTodoMutation, CreateTodoMutationVariables>;
export const UpdateTodoDocument = new TypedDocumentString(`
    mutation UpdateTodo($input: UpdateTodoInput!) {
  updateTodo(input: $input) {
    ...TodoFields
  }
}
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
}`) as unknown as TypedDocumentString<UpdateTodoMutation, UpdateTodoMutationVariables>;
export const ToggleTodoDocument = new TypedDocumentString(`
    mutation ToggleTodo($id: UUID!) {
  toggleTodoComplete(id: $id) {
    ...TodoFields
  }
}
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
}`) as unknown as TypedDocumentString<ToggleTodoMutation, ToggleTodoMutationVariables>;
export const DeleteTodoDocument = new TypedDocumentString(`
    mutation DeleteTodo($id: UUID!) {
  deleteTodo(id: $id) {
    success
  }
}
    `) as unknown as TypedDocumentString<DeleteTodoMutation, DeleteTodoMutationVariables>;
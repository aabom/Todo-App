using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;

namespace TodoApp.Api.GraphQL.Mutations;

/// <summary>
/// GraphQL Mutation root type.
/// All write operations (create, update, delete) are defined here.
/// </summary>
public class Mutation
{
    /// <summary>
    /// Create a new todo item
    /// </summary>
    [GraphQLDescription("Create a new todo item")]
    public async Task<TodoItemDto> CreateTodo(
        CreateTodoInput input,
        [Service] ITodoService todoService)
    {
        return await todoService.CreateAsync(input);
    }

    /// <summary>
    /// Update an existing todo item
    /// </summary>
    [GraphQLDescription("Update an existing todo item")]
    public async Task<TodoItemDto?> UpdateTodo(
        UpdateTodoInput input,
        [Service] ITodoService todoService)
    {
        return await todoService.UpdateAsync(input);
    }

    /// <summary>
    /// Delete a todo item
    /// </summary>
    [GraphQLDescription("Delete a todo item by ID")]
    public async Task<DeleteTodoPayload> DeleteTodo(
        Guid id,
        [Service] ITodoService todoService)
    {
        var success = await todoService.DeleteAsync(id);
        return new DeleteTodoPayload(success, id);
    }

    /// <summary>
    /// Toggle the completion status of a todo
    /// </summary>
    [GraphQLDescription("Toggle the completion status of a todo item")]
    public async Task<TodoItemDto?> ToggleTodoComplete(
        Guid id,
        [Service] ITodoService todoService)
    {
        return await todoService.ToggleCompleteAsync(id);
    }
}

/// <summary>
/// Payload for delete operation
/// </summary>
public record DeleteTodoPayload(bool Success, Guid DeletedId);

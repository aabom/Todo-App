using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;

namespace TodoApp.Api.GraphQL.Queries;

/// <summary>
/// GraphQL Query root type.
/// All read operations are defined here.
/// </summary>
public class Query
{
    /// <summary>
    /// Get all todo items
    /// </summary>
    [GraphQLDescription("Get all todo items, ordered by priority and due date")]
    public async Task<IReadOnlyList<TodoItemDto>> GetTodos(
        [Service] ITodoService todoService)
    {
        return await todoService.GetAllAsync();
    }

    /// <summary>
    /// Get a single todo by ID
    /// </summary>
    [GraphQLDescription("Get a specific todo item by its ID")]
    public async Task<TodoItemDto?> GetTodo(
        Guid id,
        [Service] ITodoService todoService)
    {
        return await todoService.GetByIdAsync(id);
    }

    /// <summary>
    /// Get todos by completion status
    /// </summary>
    [GraphQLDescription("Get todos filtered by completion status")]
    public async Task<IReadOnlyList<TodoItemDto>> GetTodosByStatus(
        bool isCompleted,
        [Service] ITodoService todoService)
    {
        return await todoService.GetByStatusAsync(isCompleted);
    }

    /// <summary>
    /// Get overdue todos
    /// </summary>
    [GraphQLDescription("Get all overdue (past due date and not completed) todos")]
    public async Task<IReadOnlyList<TodoItemDto>> GetOverdueTodos(
        [Service] ITodoService todoService)
    {
        return await todoService.GetOverdueAsync();
    }

    /// <summary>
    /// Get todo statistics
    /// </summary>
    [GraphQLDescription("Get aggregate statistics about todos")]
    public async Task<TodoStatisticsDto> GetStatistics(
        [Service] ITodoService todoService)
    {
        return await todoService.GetStatisticsAsync();
    }
}

using TodoApp.Application.DTOs;

namespace TodoApp.Application.Interfaces;

/// <summary>
/// Application service interface. Defines use cases.
/// This is the boundary between presentation and business logic.
/// </summary>
public interface ITodoService
{
    // Queries
    Task<TodoItemDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<TodoItemDto>> GetAllAsync();
    Task<IReadOnlyList<TodoItemDto>> GetByStatusAsync(bool isCompleted);
    Task<IReadOnlyList<TodoItemDto>> GetOverdueAsync();
    Task<TodoStatisticsDto> GetStatisticsAsync();

    // Commands
    Task<TodoItemDto> CreateAsync(CreateTodoInput input);
    Task<TodoItemDto?> UpdateAsync(UpdateTodoInput input);
    Task<bool> DeleteAsync(Guid id);
    Task<TodoItemDto?> ToggleCompleteAsync(Guid id);
}

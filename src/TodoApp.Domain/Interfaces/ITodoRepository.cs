using TodoApp.Domain.Models;

namespace TodoApp.Domain.Interfaces;

/// <summary>
/// Defined in Domain layer but implemented in Infrastructure (Dependency Inversion Principle).
/// </summary>
public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<TodoItem>> GetAllAsync();
    Task<IReadOnlyList<TodoItem>> GetByStatusAsync(bool isCompleted);
    Task<IReadOnlyList<TodoItem>> GetOverdueAsync();
    Task<TodoItem> AddAsync(TodoItem item);
    Task UpdateAsync(TodoItem item);
    Task DeleteAsync(Guid id);
    Task<int> GetCountAsync(bool? isCompleted = null);
}

using TodoApp.Domain.Models;

namespace TodoApp.Domain.Interfaces;

/// <summary>
/// Defined in Domain layer but implemented in Infrastructure (Dependency Inversion Principle).
/// Lists come back in display order: highest priority first, then earliest due date
/// (items without one last), then newest first.
/// </summary>
public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<TodoItem>> GetAllAsync();
    Task<IReadOnlyList<TodoItem>> GetByStatusAsync(bool isCompleted);
    Task<IReadOnlyList<TodoItem>> GetOverdueAsync(DateTime now);
    Task<int> GetOverdueCountAsync(DateTime now);
    Task<TodoItem> AddAsync(TodoItem item);
    Task UpdateAsync(TodoItem item);
    Task DeleteAsync(Guid id);
    Task<int> GetCountAsync(bool? isCompleted = null);
}

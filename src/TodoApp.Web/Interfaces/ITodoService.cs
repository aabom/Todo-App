using TodoApp.Web.Models.Todo;

namespace TodoApp.Web.Interfaces;

/// <summary>
/// Application-level service interface for todo operations.
/// </summary>
public interface ITodoService
{
    Task<List<TodoItemModel>> GetAllAsync();
    Task<TodoItemModel?> GetByIdAsync(Guid id);
    Task<TodoStatisticsModel> GetStatisticsAsync();
    Task<TodoItemModel?> CreateAsync(CreateTodoModel model);
    Task<TodoItemModel?> ToggleCompleteAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
    Task<TodoItemModel?> UpdateAsync(UpdateTodoModel model);
}

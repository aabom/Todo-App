using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Models;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Services;

/// <summary>
/// Application service implementation. Orchestrates domain objects and repositories.
/// </summary>
public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<TodoItemDto?> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item is null ? null : MapToDto(item);
    }

    public async Task<IReadOnlyList<TodoItemDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<TodoItemDto>> GetByStatusAsync(bool isCompleted)
    {
        var items = await _repository.GetByStatusAsync(isCompleted);
        return items.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<TodoItemDto>> GetOverdueAsync()
    {
        var items = await _repository.GetOverdueAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<TodoStatisticsDto> GetStatisticsAsync()
    {
        var total = await _repository.GetCountAsync();
        var completed = await _repository.GetCountAsync(true);
        var overdue = (await _repository.GetOverdueAsync()).Count;

        return new TodoStatisticsDto(
            TotalCount: total,
            CompletedCount: completed,
            PendingCount: total - completed,
            OverdueCount: overdue
        );
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoInput input)
    {
        var item = TodoItem.Create(
            title: input.Title,
            description: input.Description,
            priority: input.Priority,
            dueDate: input.DueDate
        );

        await _repository.AddAsync(item);
        return MapToDto(item);
    }

    public async Task<TodoItemDto?> UpdateAsync(UpdateTodoInput input)
    {
        var item = await _repository.GetByIdAsync(input.Id);
        if (item is null) return null;

        if (input.Title is not null)
            item.UpdateTitle(input.Title);

        if (input.Description is not null)
            item.UpdateDescription(input.Description);

        if (input.Priority.HasValue)
            item.SetPriority(input.Priority.Value);

        if (input.DueDate.HasValue)
            item.SetDueDate(input.DueDate.Value);

        await _repository.UpdateAsync(item);
        return MapToDto(item);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null) return false;

        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<TodoItemDto?> ToggleCompleteAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null) return null;

        if (item.IsCompleted)
            item.MarkAsIncomplete();
        else
            item.MarkAsComplete();

        await _repository.UpdateAsync(item);
        return MapToDto(item);
    }

    private static TodoItemDto MapToDto(TodoItem item) => new(
        Id: item.Id,
        Title: item.Title,
        Description: item.Description,
        IsCompleted: item.IsCompleted,
        Priority: item.Priority,
        CreatedAt: item.CreatedAt,
        CompletedAt: item.CompletedAt,
        DueDate: item.DueDate,
        IsOverdue: item.IsOverdue
    );
}

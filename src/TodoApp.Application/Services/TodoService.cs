using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Exceptions;
using TodoApp.Domain.Interfaces;
using TodoApp.Domain.Models;

namespace TodoApp.Application.Services;

/// <summary>
/// Application service implementation. Orchestrates domain objects and repositories.
/// The clock comes from <see cref="TimeProvider"/> so "overdue" can be tested.
/// </summary>
public class TodoService(ITodoRepository repository, TimeProvider timeProvider) : ITodoService
{
    private readonly ITodoRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    private DateTime UtcNow => _timeProvider.GetUtcNow().UtcDateTime;

    public async Task<TodoItemDto?> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item is null ? null : MapToDto(item, UtcNow);
    }

    public async Task<IReadOnlyList<TodoItemDto>> GetAllAsync() => MapAll(await _repository.GetAllAsync());

    public async Task<IReadOnlyList<TodoItemDto>> GetByStatusAsync(bool isCompleted) =>
        MapAll(await _repository.GetByStatusAsync(isCompleted));

    public async Task<IReadOnlyList<TodoItemDto>> GetOverdueAsync() => MapAll(await _repository.GetOverdueAsync(UtcNow));

    public async Task<TodoStatisticsDto> GetStatisticsAsync()
    {
        var total = await _repository.GetCountAsync();
        var completed = await _repository.GetCountAsync(true);
        var overdue = await _repository.GetOverdueCountAsync(UtcNow);

        return new TodoStatisticsDto(
            TotalCount: total,
            CompletedCount: completed,
            PendingCount: total - completed,
            OverdueCount: overdue
        );
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoInput input)
    {
        var now = UtcNow;
        var item = TodoItem.Create(
            title: input.Title,
            createdAt: now,
            description: input.Description,
            priority: input.Priority,
            dueDate: input.DueDate
        );

        await _repository.AddAsync(item);
        return MapToDto(item, now);
    }

    public async Task<TodoItemDto?> UpdateAsync(UpdateTodoInput input)
    {
        if (input.ClearDueDate && input.DueDate.HasValue)
            throw new TodoValidationException(nameof(input.DueDate), "Set either a due date or clearDueDate, not both.");

        var item = await _repository.GetByIdAsync(input.Id);
        if (item is null) return null;

        if (input.Title is not null)
            item.UpdateTitle(input.Title);

        if (input.Description is not null)
            item.UpdateDescription(input.Description);

        if (input.Priority.HasValue)
            item.SetPriority(input.Priority.Value);

        if (input.ClearDueDate)
            item.SetDueDate(null);
        else if (input.DueDate.HasValue)
            item.SetDueDate(input.DueDate.Value);

        await _repository.UpdateAsync(item);
        return MapToDto(item, UtcNow);
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

        var now = UtcNow;
        if (item.IsCompleted)
            item.MarkAsIncomplete();
        else
            item.MarkAsComplete(now);

        await _repository.UpdateAsync(item);
        return MapToDto(item, now);
    }

    private IReadOnlyList<TodoItemDto> MapAll(IEnumerable<TodoItem> items)
    {
        var now = UtcNow;
        return items.Select(item => MapToDto(item, now)).ToList();
    }

    private static TodoItemDto MapToDto(TodoItem item, DateTime now) => new(
        Id: item.Id,
        Title: item.Title,
        Description: item.Description,
        IsCompleted: item.IsCompleted,
        Priority: item.Priority,
        CreatedAt: item.CreatedAt,
        CompletedAt: item.CompletedAt,
        DueDate: item.DueDate,
        IsOverdue: item.IsOverdueAt(now)
    );
}

using TodoApp.Domain.Models;

namespace TodoApp.Application.DTOs;

public record TodoItemDto(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    Priority Priority,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    DateTime? DueDate,
    bool IsOverdue
);

public record CreateTodoInput(
    string Title,
    string? Description = null,
    Priority Priority = Priority.Medium,
    DateTime? DueDate = null
);

/// <summary>
/// A partial update: a field left null is left unchanged.
/// To remove a value, send an empty <see cref="Description"/>, or set
/// <see cref="ClearDueDate"/> (a null <see cref="DueDate"/> alone means "unchanged").
/// </summary>
public record UpdateTodoInput(
    Guid Id,
    string? Title = null,
    string? Description = null,
    Priority? Priority = null,
    DateTime? DueDate = null,
    bool ClearDueDate = false
);

public record TodoStatisticsDto(
    int TotalCount,
    int CompletedCount,
    int PendingCount,
    int OverdueCount
);

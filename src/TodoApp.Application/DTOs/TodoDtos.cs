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

public record UpdateTodoInput(
    Guid Id,
    string? Title = null,
    string? Description = null,
    Priority? Priority = null,
    DateTime? DueDate = null
);

public record TodoStatisticsDto(
    int TotalCount,
    int CompletedCount,
    int PendingCount,
    int OverdueCount
);

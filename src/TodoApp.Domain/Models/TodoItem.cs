using TodoApp.Domain.Exceptions;

namespace TodoApp.Domain.Models;

/// <summary>
/// Core domain model representing a todo item. All changes go through methods,
/// so the rules (a title is required, lengths are bounded) cannot be bypassed.
/// Time is always passed in rather than read from the clock, which keeps the
/// model deterministic and testable.
/// </summary>
public class TodoItem
{
    public const int TitleMaxLength = 200;
    public const int DescriptionMaxLength = 1000;

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsCompleted { get; private set; }
    public Priority Priority { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? DueDate { get; private set; }

    private TodoItem() { }

    public static TodoItem Create(
        string title,
        DateTime createdAt,
        string? description = null,
        Priority priority = Priority.Medium,
        DateTime? dueDate = null)
    {
        return new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = NormalizeTitle(title),
            Description = NormalizeDescription(description),
            IsCompleted = false,
            Priority = priority,
            CreatedAt = createdAt,
            DueDate = dueDate
        };
    }

    public void UpdateTitle(string title) => Title = NormalizeTitle(title);

    /// <summary>Sets the description. Null, empty or whitespace clears it.</summary>
    public void UpdateDescription(string? description) => Description = NormalizeDescription(description);

    public void SetPriority(Priority priority) => Priority = priority;

    /// <summary>Sets the due date. Null clears it.</summary>
    public void SetDueDate(DateTime? dueDate) => DueDate = dueDate;

    public void MarkAsComplete(DateTime completedAt)
    {
        if (IsCompleted) return;

        IsCompleted = true;
        CompletedAt = completedAt;
    }

    public void MarkAsIncomplete()
    {
        IsCompleted = false;
        CompletedAt = null;
    }

    /// <summary>An open item whose due date has passed at <paramref name="now"/>.</summary>
    public bool IsOverdueAt(DateTime now) => DueDate.HasValue && !IsCompleted && DueDate.Value < now;

    private static string NormalizeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new TodoValidationException(nameof(Title), "Title cannot be empty.");

        var trimmed = title.Trim();
        if (trimmed.Length > TitleMaxLength)
            throw new TodoValidationException(nameof(Title), $"Title cannot be longer than {TitleMaxLength} characters.");

        return trimmed;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description)) return null;

        var trimmed = description.Trim();
        if (trimmed.Length > DescriptionMaxLength)
            throw new TodoValidationException(nameof(Description), $"Description cannot be longer than {DescriptionMaxLength} characters.");

        return trimmed;
    }
}

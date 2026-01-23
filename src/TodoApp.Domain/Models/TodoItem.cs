namespace TodoApp.Domain.Models;

/// <summary>
/// Core domain model representing a todo item.
/// </summary>
public class TodoItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsCompleted { get; private set; }
    public Priority Priority { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? DueDate { get; private set; }

    private TodoItem() { }

    public static TodoItem Create(string title, string? description = null, Priority priority = Priority.Medium, DateTime? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        return new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = description?.Trim(),
            IsCompleted = false,
            Priority = priority,
            CreatedAt = DateTime.UtcNow,
            DueDate = dueDate
        };
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        
        Title = title.Trim();
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
    }

    public void SetPriority(Priority priority)
    {
        Priority = priority;
    }

    public void SetDueDate(DateTime? dueDate)
    {
        DueDate = dueDate;
    }

    public void MarkAsComplete()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
        }
    }

    public void MarkAsIncomplete()
    {
        IsCompleted = false;
        CompletedAt = null;
    }

    public bool IsOverdue => DueDate.HasValue && !IsCompleted && DueDate.Value < DateTime.UtcNow;
}

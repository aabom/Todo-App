using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Models;

namespace TodoApp.Infrastructure.Persistence;

/// <summary>
/// A handful of sample todos for demos, so a fresh database does not open on an empty page.
/// Dates are relative to now, so there is always one overdue item and the rest are upcoming.
/// Seeding only happens into an empty table, so it never touches real data.
/// </summary>
public static class DemoData
{
    public static async Task<bool> SeedIfEmptyAsync(TodoDbContext context, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        if (await context.TodoItems.AnyAsync(cancellationToken))
            return false;

        context.TodoItems.AddRange(
            TodoItem.Create("Prepare sprint demo", utcNow, "Slides and a short live walkthrough", Priority.Critical, utcNow.AddDays(2)),
            TodoItem.Create("Review pull request", utcNow, priority: Priority.High, dueDate: utcNow.AddHours(-3)),
            TodoItem.Create("Update dependencies", utcNow, "Move to the latest patch releases", Priority.Medium),
            TodoItem.Create("Write release notes", utcNow, priority: Priority.Medium),
            TodoItem.Create("Book dentist appointment", utcNow, priority: Priority.Low, dueDate: utcNow.AddDays(14)));

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

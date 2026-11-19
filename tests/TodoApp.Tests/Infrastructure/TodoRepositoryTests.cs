using TodoApp.Domain.Models;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Tests.Support;

namespace TodoApp.Tests.Infrastructure;

public class TodoRepositoryTests : IDisposable
{
    private static readonly DateTime Now = new(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);

    private readonly SqliteTestDatabase _database = new();

    public void Dispose() => _database.Dispose();

    private async Task SeedAsync(params TodoItem[] items)
    {
        await using var context = _database.CreateContext();
        var repository = new TodoRepository(context);
        foreach (var item in items)
            await repository.AddAsync(item);
    }

    private TodoRepository FreshRepository() => new(_database.CreateContext());

    [Fact]
    public async Task GetAll_OrdersByPriorityThenDueDateWithUndatedItemsLast()
    {
        var undated = TodoItem.Create("Undated", Now, priority: Priority.High);
        var later = TodoItem.Create("Later", Now, priority: Priority.High, dueDate: Now.AddDays(5));
        var sooner = TodoItem.Create("Sooner", Now, priority: Priority.High, dueDate: Now.AddDays(1));
        var critical = TodoItem.Create("Critical", Now, priority: Priority.Critical);
        var low = TodoItem.Create("Low", Now, priority: Priority.Low, dueDate: Now);
        await SeedAsync(undated, later, low, sooner, critical);

        var titles = (await FreshRepository().GetAllAsync()).Select(t => t.Title);

        Assert.Equal(["Critical", "Sooner", "Later", "Undated", "Low"], titles);
    }

    [Fact]
    public async Task GetAll_BreaksTiesWithNewestFirst()
    {
        await SeedAsync(
            TodoItem.Create("Older", Now),
            TodoItem.Create("Newer", Now.AddMinutes(1)));

        var titles = (await FreshRepository().GetAllAsync()).Select(t => t.Title);

        Assert.Equal(["Newer", "Older"], titles);
    }

    [Fact]
    public async Task Overdue_CountsOnlyOpenItemsPastTheirDueDate()
    {
        var overdue = TodoItem.Create("Overdue", Now, dueDate: Now.AddHours(-1));
        var done = TodoItem.Create("Done", Now, dueDate: Now.AddHours(-1));
        done.MarkAsComplete(Now);
        var future = TodoItem.Create("Future", Now, dueDate: Now.AddHours(1));
        var undated = TodoItem.Create("Undated", Now);
        await SeedAsync(overdue, done, future, undated);

        var repository = FreshRepository();

        Assert.Equal(1, await repository.GetOverdueCountAsync(Now));
        Assert.Equal("Overdue", Assert.Single(await repository.GetOverdueAsync(Now)).Title);
    }

    [Fact]
    public async Task DatesComeBackMarkedAsUtc()
    {
        // SQLite stores no time zone. Without the converters in TodoDbContext the values
        // come back as Unspecified and the client would show them in the wrong zone.
        var item = TodoItem.Create("Task", Now, dueDate: Now.AddDays(1));
        item.MarkAsComplete(Now);
        await SeedAsync(item);

        var loaded = await FreshRepository().GetByIdAsync(item.Id);

        Assert.NotNull(loaded);
        Assert.Equal(DateTimeKind.Utc, loaded.CreatedAt.Kind);
        Assert.Equal(DateTimeKind.Utc, loaded.DueDate!.Value.Kind);
        Assert.Equal(DateTimeKind.Utc, loaded.CompletedAt!.Value.Kind);
    }

    [Fact]
    public async Task Delete_RemovesTheItem()
    {
        var item = TodoItem.Create("Task", Now);
        await SeedAsync(item);

        await FreshRepository().DeleteAsync(item.Id);

        Assert.Null(await FreshRepository().GetByIdAsync(item.Id));
    }
}

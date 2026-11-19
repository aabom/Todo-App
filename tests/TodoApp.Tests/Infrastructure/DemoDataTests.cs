using TodoApp.Domain.Models;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Tests.Support;

namespace TodoApp.Tests.Infrastructure;

public class DemoDataTests : IDisposable
{
    private static readonly DateTime Now = new(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);

    private readonly SqliteTestDatabase _database = new();

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task SeedsAnEmptyDatabaseWithOneOverdueItem()
    {
        await using (var context = _database.CreateContext())
            Assert.True(await DemoData.SeedIfEmptyAsync(context, Now, TestContext.Current.CancellationToken));

        await using var check = _database.CreateContext();
        var items = check.TodoItems.ToList();
        Assert.Equal(5, items.Count);
        Assert.Single(items, t => t.IsOverdueAt(Now));
    }

    [Fact]
    public async Task LeavesExistingDataAlone()
    {
        await using (var context = _database.CreateContext())
        {
            context.TodoItems.Add(TodoItem.Create("Mine", Now));
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using (var context = _database.CreateContext())
            Assert.False(await DemoData.SeedIfEmptyAsync(context, Now, TestContext.Current.CancellationToken));

        await using var check = _database.CreateContext();
        Assert.Equal("Mine", Assert.Single(check.TodoItems.ToList()).Title);
    }
}

using TodoApp.Domain.Exceptions;
using TodoApp.Domain.Models;

namespace TodoApp.Tests.Domain;

public class TodoItemTests
{
    private static readonly DateTime Now = new(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_TrimsTitleAndDescription()
    {
        var item = TodoItem.Create("  Buy milk  ", Now, "  two litres  ");

        Assert.Equal("Buy milk", item.Title);
        Assert.Equal("two litres", item.Description);
        Assert.Equal(Now, item.CreatedAt);
        Assert.False(item.IsCompleted);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_RejectsBlankTitle(string title)
    {
        var error = Assert.Throws<TodoValidationException>(() => TodoItem.Create(title, Now));

        Assert.Equal(nameof(TodoItem.Title), error.Field);
    }

    [Fact]
    public void Create_RejectsTitleLongerThanTheLimit()
    {
        var title = new string('a', TodoItem.TitleMaxLength + 1);

        Assert.Throws<TodoValidationException>(() => TodoItem.Create(title, Now));
    }

    [Fact]
    public void Create_AcceptsTitleAtTheLimit()
    {
        var title = new string('a', TodoItem.TitleMaxLength);

        Assert.Equal(title, TodoItem.Create(title, Now).Title);
    }

    [Fact]
    public void UpdateDescription_WithWhitespace_ClearsIt()
    {
        var item = TodoItem.Create("Task", Now, "something");

        item.UpdateDescription("   ");

        Assert.Null(item.Description);
    }

    [Fact]
    public void UpdateDescription_RejectsTextLongerThanTheLimit()
    {
        var item = TodoItem.Create("Task", Now);

        Assert.Throws<TodoValidationException>(() =>
            item.UpdateDescription(new string('a', TodoItem.DescriptionMaxLength + 1)));
    }

    [Fact]
    public void MarkAsComplete_Twice_KeepsTheFirstCompletionTime()
    {
        var item = TodoItem.Create("Task", Now);

        item.MarkAsComplete(Now.AddHours(1));
        item.MarkAsComplete(Now.AddHours(2));

        Assert.Equal(Now.AddHours(1), item.CompletedAt);
    }

    [Fact]
    public void MarkAsIncomplete_ClearsCompletionTime()
    {
        var item = TodoItem.Create("Task", Now);
        item.MarkAsComplete(Now);

        item.MarkAsIncomplete();

        Assert.False(item.IsCompleted);
        Assert.Null(item.CompletedAt);
    }

    [Fact]
    public void IsOverdueAt_IsTrueOnlyForOpenItemsPastTheirDueDate()
    {
        var item = TodoItem.Create("Task", Now, dueDate: Now.AddHours(1));

        Assert.False(item.IsOverdueAt(Now));
        Assert.False(item.IsOverdueAt(Now.AddHours(1)));
        Assert.True(item.IsOverdueAt(Now.AddHours(2)));

        item.MarkAsComplete(Now.AddHours(2));
        Assert.False(item.IsOverdueAt(Now.AddHours(2)));
    }

    [Fact]
    public void IsOverdueAt_IsFalseWithoutADueDate()
    {
        Assert.False(TodoItem.Create("Task", Now).IsOverdueAt(DateTime.MaxValue));
    }
}

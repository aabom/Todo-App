using Microsoft.Extensions.Time.Testing;
using TodoApp.Application.DTOs;
using TodoApp.Application.Services;
using TodoApp.Domain.Exceptions;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Tests.Support;

namespace TodoApp.Tests.Application;

public class TodoServiceTests : IDisposable
{
    private static readonly DateTimeOffset Start = new(2026, 1, 15, 12, 0, 0, TimeSpan.Zero);

    private readonly SqliteTestDatabase _database = new();
    private readonly TodoDbContext _context;
    private readonly FakeTimeProvider _clock = new(Start);
    private readonly TodoService _service;

    public TodoServiceTests()
    {
        _context = _database.CreateContext();
        _service = new TodoService(new TodoRepository(_context), _clock);
    }

    public void Dispose()
    {
        _context.Dispose();
        _database.Dispose();
    }

    private Task<TodoItemDto> CreateAsync(string? description = "notes", DateTime? dueDate = null) =>
        _service.CreateAsync(new CreateTodoInput("Task", description, DueDate: dueDate));

    [Fact]
    public async Task Create_StampsCreationTimeFromTheClock()
    {
        var created = await CreateAsync();

        Assert.Equal(Start.UtcDateTime, created.CreatedAt);
    }

    [Fact]
    public async Task Update_WithClearDueDate_RemovesTheDueDate()
    {
        // Regression: a null due date used to mean "unchanged", so there was no way to remove one.
        var created = await CreateAsync(dueDate: Start.UtcDateTime.AddDays(1));

        var updated = await _service.UpdateAsync(new UpdateTodoInput(created.Id, ClearDueDate: true));

        Assert.NotNull(updated);
        Assert.Null(updated.DueDate);
        Assert.Null((await _service.GetByIdAsync(created.Id))!.DueDate);
    }

    [Fact]
    public async Task Update_WithEmptyDescription_RemovesTheDescription()
    {
        var created = await CreateAsync(description: "notes");

        var updated = await _service.UpdateAsync(new UpdateTodoInput(created.Id, Description: ""));

        Assert.Null(updated!.Description);
    }

    [Fact]
    public async Task Update_LeavesFieldsThatWereNotSentUnchanged()
    {
        var dueDate = Start.UtcDateTime.AddDays(1);
        var created = await CreateAsync(description: "notes", dueDate: dueDate);

        var updated = await _service.UpdateAsync(new UpdateTodoInput(created.Id, Title: "Renamed"));

        Assert.Equal("Renamed", updated!.Title);
        Assert.Equal("notes", updated.Description);
        Assert.Equal(dueDate, updated.DueDate);
    }

    [Fact]
    public async Task Update_RejectsANewDueDateTogetherWithClearDueDate()
    {
        var created = await CreateAsync();

        await Assert.ThrowsAsync<TodoValidationException>(() => _service.UpdateAsync(
            new UpdateTodoInput(created.Id, DueDate: Start.UtcDateTime, ClearDueDate: true)));
    }

    [Fact]
    public async Task Update_ReturnsNullForAnUnknownId()
    {
        Assert.Null(await _service.UpdateAsync(new UpdateTodoInput(Guid.NewGuid(), Title: "x")));
    }

    [Fact]
    public async Task Toggle_StampsCompletionTimeAndCanBeUndone()
    {
        var created = await CreateAsync();
        _clock.Advance(TimeSpan.FromHours(1));

        var completed = await _service.ToggleCompleteAsync(created.Id);
        var reopened = await _service.ToggleCompleteAsync(created.Id);

        Assert.Equal(Start.UtcDateTime.AddHours(1), completed!.CompletedAt);
        Assert.False(reopened!.IsCompleted);
        Assert.Null(reopened.CompletedAt);
    }

    [Fact]
    public async Task Statistics_CountOverdueItemsAgainstTheClock()
    {
        await CreateAsync(dueDate: Start.UtcDateTime.AddHours(1));
        var done = await CreateAsync();
        await _service.ToggleCompleteAsync(done.Id);

        Assert.Equal(new TodoStatisticsDto(2, 1, 1, 0), await _service.GetStatisticsAsync());

        _clock.Advance(TimeSpan.FromHours(2));

        Assert.Equal(new TodoStatisticsDto(2, 1, 1, 1), await _service.GetStatisticsAsync());
        Assert.True(Assert.Single(await _service.GetOverdueAsync()).IsOverdue);
    }

    [Fact]
    public async Task Delete_ReportsWhetherSomethingWasDeleted()
    {
        var created = await CreateAsync();

        Assert.True(await _service.DeleteAsync(created.Id));
        Assert.False(await _service.DeleteAsync(created.Id));
    }
}

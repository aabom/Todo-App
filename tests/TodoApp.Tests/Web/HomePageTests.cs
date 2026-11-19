using Bunit;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Domain.Models;
using TodoApp.Web.Interfaces;
using TodoApp.Web.Models.Todo;
using TodoApp.Web.Pages;

namespace TodoApp.Tests.Web;

public class HomePageTests : BunitContext
{
    private readonly FakeTodoService _todos = new();

    public HomePageTests() => Services.AddSingleton<ITodoService>(_todos);

    private static IEnumerable<string> Titles(IRenderedComponent<Home> page) =>
        page.FindAll(".todo-title").Select(e => e.TextContent);

    [Fact]
    public void AddedTodo_TakesItsPlaceInServerOrder()
    {
        // Regression: a new todo was always inserted at the top, whatever its priority.
        _todos.Seed(Item("Urgent", Priority.Critical));
        var page = Render<Home>();

        page.Find(".add-todo-form .title-input").Input("Someday");
        page.Find(".add-todo-form select").Change(Priority.Low.ToString());
        page.Find(".add-btn").Click();

        Assert.Equal(["Urgent", "Someday"], Titles(page));
    }

    [Fact]
    public void Editing_WithTheDueDateEmptied_AsksTheServerToClearIt()
    {
        _todos.Seed(Item("Task", Priority.Medium, dueDate: DateTime.UtcNow.AddDays(1), description: "notes"));
        var page = Render<Home>();

        page.Find(".edit-btn").Click();
        page.Find(".todo-edit-form input[type=datetime-local]").Change("");
        page.Find(".todo-edit-form .description-input").Change("");
        page.Find(".save-btn").Click();

        var sent = Assert.Single(_todos.Updates);
        Assert.True(sent.ClearDueDate);
        Assert.Null(sent.DueDate);
        Assert.Equal(string.Empty, sent.Description);
    }

    [Fact]
    public void Editing_WithADueDate_SendsItAndDoesNotClear()
    {
        _todos.Seed(Item("Task", Priority.Medium, dueDate: DateTime.UtcNow.AddDays(1)));
        var page = Render<Home>();

        page.Find(".edit-btn").Click();
        page.Find(".save-btn").Click();

        var sent = Assert.Single(_todos.Updates);
        Assert.False(sent.ClearDueDate);
        Assert.NotNull(sent.DueDate);
    }

    private static TodoItemModel Item(string title, Priority priority, DateTime? dueDate = null, string? description = null) => new()
    {
        Id = Guid.NewGuid(),
        Title = title,
        Description = description,
        Priority = priority,
        CreatedAt = DateTime.UtcNow,
        DueDate = dueDate
    };

    /// <summary>Stands in for the GraphQL client and sorts the way the server does.</summary>
    private sealed class FakeTodoService : ITodoService
    {
        private readonly List<TodoItemModel> _items = [];

        public List<UpdateTodoModel> Updates { get; } = [];

        public void Seed(params TodoItemModel[] items) => _items.AddRange(items);

        public Task<List<TodoItemModel>> GetAllAsync() => Task.FromResult(_items
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate is null)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToList());

        public Task<TodoItemModel?> GetByIdAsync(Guid id) => Task.FromResult(_items.FirstOrDefault(t => t.Id == id));

        public Task<TodoStatisticsModel> GetStatisticsAsync() => Task.FromResult(new TodoStatisticsModel
        {
            TotalCount = _items.Count,
            CompletedCount = _items.Count(t => t.IsCompleted),
            PendingCount = _items.Count(t => !t.IsCompleted)
        });

        public Task<TodoItemModel?> CreateAsync(CreateTodoModel model)
        {
            var item = Item(model.Title, model.Priority, model.DueDate, model.Description);
            _items.Add(item);
            return Task.FromResult<TodoItemModel?>(item);
        }

        public Task<TodoItemModel?> UpdateAsync(UpdateTodoModel model)
        {
            Updates.Add(model);
            return GetByIdAsync(model.Id);
        }

        public Task<TodoItemModel?> ToggleCompleteAsync(Guid id) => GetByIdAsync(id);

        public Task<bool> DeleteAsync(Guid id) => Task.FromResult(_items.RemoveAll(t => t.Id == id) > 0);
    }
}

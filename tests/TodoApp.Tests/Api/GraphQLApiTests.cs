using System.Text.Json;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using TodoApp.Api.GraphQL;
using TodoApp.Api.GraphQL.Errors;
using TodoApp.Application;
using TodoApp.Domain.Interfaces;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Tests.Support;

namespace TodoApp.Tests.Api;

/// <summary>
/// Runs operations through the same schema the API serves (<see cref="GraphQLSetup"/>),
/// against a real SQLite database, without starting a web server.
/// </summary>
public class GraphQLApiTests : IAsyncLifetime
{
    private readonly SqliteTestDatabase _database = new();
    private ServiceProvider _services = null!;
    private IRequestExecutor _executor = null!;

    public async ValueTask InitializeAsync()
    {
        var services = new ServiceCollection();
        services.AddSingleton<TimeProvider>(new FakeTimeProvider(new DateTimeOffset(2026, 1, 15, 12, 0, 0, TimeSpan.Zero)));
        services.AddApplication();
        services.AddDbContext<TodoDbContext>(o => o.UseSqlite(_database.Connection));
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddTodoGraphQL();

        _services = services.BuildServiceProvider();
        _executor = await _services.GetRequiredService<IRequestExecutorResolver>().GetRequestExecutorAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _services.DisposeAsync();
        _database.Dispose();
    }

    private async Task<JsonElement> ExecuteAsync(string document, Dictionary<string, object?>? variables = null)
    {
        await using var scope = _services.CreateAsyncScope();
        var request = OperationRequestBuilder.New()
            .SetDocument(document)
            .SetVariableValues(variables)
            .SetServices(scope.ServiceProvider)
            .Build();

        var result = await _executor.ExecuteAsync(request);
        return JsonDocument.Parse(result.ToJson()).RootElement.Clone();
    }

    private async Task<string> CreateTodoAsync(string dueDate)
    {
        var json = await ExecuteAsync(
            $$"""mutation { createTodo(input: { title: "Task", dueDate: "{{dueDate}}" }) { id } }""");
        return json.GetProperty("data").GetProperty("createTodo").GetProperty("id").GetString()!;
    }

    [Fact]
    public async Task CreateTodo_WithBlankTitle_ReturnsAReadableValidationError()
    {
        // Regression: this used to surface as "Unexpected Execution Error" with no reason.
        var json = await ExecuteAsync("""mutation { createTodo(input: { title: "   " }) { id } }""");

        var error = json.GetProperty("errors")[0];
        Assert.Equal("Title cannot be empty.", error.GetProperty("message").GetString());
        Assert.Equal(TodoValidationErrorFilter.Code, error.GetProperty("extensions").GetProperty("code").GetString());
        Assert.Equal("Title", error.GetProperty("extensions").GetProperty("field").GetString());
    }

    [Fact]
    public async Task UpdateTodo_WithClearDueDate_RemovesTheDueDate()
    {
        var id = await CreateTodoAsync("2030-01-01T00:00:00Z");

        var json = await ExecuteAsync(
            """mutation($id: UUID!) { updateTodo(input: { id: $id, clearDueDate: true }) { dueDate } }""",
            new() { ["id"] = id });

        Assert.Equal(JsonValueKind.Null, json.GetProperty("data").GetProperty("updateTodo").GetProperty("dueDate").ValueKind);
    }

    [Fact]
    public async Task UpdateTodo_WithoutDueDateFields_KeepsTheDueDate()
    {
        var id = await CreateTodoAsync("2030-01-01T00:00:00Z");

        var json = await ExecuteAsync(
            """mutation($id: UUID!) { updateTodo(input: { id: $id, title: "Renamed" }) { title dueDate } }""",
            new() { ["id"] = id });

        var todo = json.GetProperty("data").GetProperty("updateTodo");
        Assert.Equal("Renamed", todo.GetProperty("title").GetString());
        Assert.Equal("2030-01-01T00:00:00.000Z", todo.GetProperty("dueDate").GetString());
    }

    [Fact]
    public async Task Statistics_ReflectWhatWasCreated()
    {
        await CreateTodoAsync("2020-01-01T00:00:00Z");

        var json = await ExecuteAsync("{ statistics { totalCount pendingCount overdueCount } }");

        var stats = json.GetProperty("data").GetProperty("statistics");
        Assert.Equal(1, stats.GetProperty("totalCount").GetInt32());
        Assert.Equal(1, stats.GetProperty("pendingCount").GetInt32());
        Assert.Equal(1, stats.GetProperty("overdueCount").GetInt32());
    }
}

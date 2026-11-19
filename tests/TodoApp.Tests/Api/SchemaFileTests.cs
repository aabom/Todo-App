using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Api.GraphQL;
using TodoApp.Application;

namespace TodoApp.Tests.Api;

/// <summary>
/// <c>src/TodoApp.Api/schema.graphql</c> is the schema the React client's types are generated
/// from. This test fails when the C# types change and the file has not been updated, so the
/// generated TypeScript can never describe an API that no longer exists.
/// Set <c>UPDATE_SCHEMA=1</c> and run the test to rewrite the file.
/// </summary>
public class SchemaFileTests
{
    [Fact]
    public async Task SchemaFileMatchesTheServedSchema()
    {
        var services = new ServiceCollection();
        services.AddSingleton(TimeProvider.System);
        services.AddApplication();
        services.AddTodoGraphQL();
        await using var provider = services.BuildServiceProvider();

        var executor = await provider.GetRequiredService<IRequestExecutorResolver>()
            .GetRequestExecutorAsync(cancellationToken: TestContext.Current.CancellationToken);
        var served = Normalize(executor.Schema.ToString());

        var path = Path.Combine(RepositoryRoot(), "src", "TodoApp.Api", "schema.graphql");

        if (Environment.GetEnvironmentVariable("UPDATE_SCHEMA") == "1")
        {
            await File.WriteAllTextAsync(path, served, TestContext.Current.CancellationToken);
            return;
        }

        var committed = File.Exists(path)
            ? Normalize(await File.ReadAllTextAsync(path, TestContext.Current.CancellationToken))
            : "";
        Assert.True(
            committed == served,
            $"{path} is out of date. Run `UPDATE_SCHEMA=1 dotnet test`, then `npm run codegen` in src/TodoApp.WebReact.");
    }

    private static string Normalize(string sdl) => sdl.ReplaceLineEndings("\n").TrimEnd() + "\n";

    private static string RepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "TodoApp.sln")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("TodoApp.sln not found above the test output.");
    }
}

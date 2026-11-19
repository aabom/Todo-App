namespace TodoApp.Web.Interfaces;

/// <summary>
/// GraphQL client for communicating with the API. Queries and mutations travel the same way
/// (a POST to /graphql), so one method serves both.
/// </summary>
public interface IGraphQLService
{
    /// <summary>Runs a query or mutation and returns its <c>data</c>. GraphQL errors throw <see cref="Models.GraphQL.GraphQLException"/>.</summary>
    Task<T?> ExecuteAsync<T>(string document, object? variables = null);
}

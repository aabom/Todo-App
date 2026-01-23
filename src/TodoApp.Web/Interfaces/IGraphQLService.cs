namespace TodoApp.Web.Interfaces;

/// <summary>
/// GraphQL client service interface for communicating with the API.
/// </summary>
public interface IGraphQLService
{
    Task<T?> ExecuteQueryAsync<T>(string query, object? variables = null);
    Task<T?> ExecuteMutationAsync<T>(string mutation, object? variables = null);
}

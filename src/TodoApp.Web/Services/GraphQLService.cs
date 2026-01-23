using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TodoApp.Web.Interfaces;
using TodoApp.Web.Models.GraphQL;

namespace TodoApp.Web.Services;

/// <summary>
/// GraphQL client service for communicating with the API.
/// </summary>
public class GraphQLService : IGraphQLService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public GraphQLService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<T?> ExecuteQueryAsync<T>(string query, object? variables = null)
    {
        var request = new GraphQLRequest { Query = query, Variables = variables };
        var response = await SendRequestAsync<T>(request);
        return response;
    }

    public async Task<T?> ExecuteMutationAsync<T>(string mutation, object? variables = null)
    {
        var request = new GraphQLRequest { Query = mutation, Variables = variables };
        var response = await SendRequestAsync<T>(request);
        return response;
    }

    private async Task<T?> SendRequestAsync<T>(GraphQLRequest request)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("graphql", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var graphQLResponse = JsonSerializer.Deserialize<GraphQLResponse<T>>(responseJson, _jsonOptions);

        if (graphQLResponse?.Errors?.Any() == true)
        {
            throw new GraphQLException(graphQLResponse.Errors);
        }

        return graphQLResponse != null ? graphQLResponse.Data : default;
    }
}


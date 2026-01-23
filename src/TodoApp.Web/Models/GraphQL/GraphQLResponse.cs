using System.Text.Json.Serialization;

namespace TodoApp.Web.Models.GraphQL;

public class GraphQLResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("errors")]
    public List<GraphQLError>? Errors { get; set; }
}

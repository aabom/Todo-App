using System.Text.Json.Serialization;

namespace TodoApp.Web.Models.GraphQL;

public class GraphQLError
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

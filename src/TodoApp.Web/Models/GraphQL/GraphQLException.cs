namespace TodoApp.Web.Models.GraphQL;

public class GraphQLException : Exception
{
    public List<GraphQLError> Errors { get; }

    public GraphQLException(List<GraphQLError> errors)
        : base(string.Join("; ", errors.Select(e => e.Message)))
    {
        Errors = errors;
    }
}

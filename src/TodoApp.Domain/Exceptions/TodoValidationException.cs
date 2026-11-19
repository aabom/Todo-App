namespace TodoApp.Domain.Exceptions;

/// <summary>
/// Thrown when a todo would break one of its own rules (an empty title, a title or
/// description that is too long). The API turns it into a GraphQL error the client
/// can show as-is, instead of a generic "Unexpected Execution Error".
/// </summary>
public class TodoValidationException(string field, string message) : Exception(message)
{
    public string Field { get; } = field;
}

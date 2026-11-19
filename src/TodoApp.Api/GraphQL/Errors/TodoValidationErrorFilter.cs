using TodoApp.Domain.Exceptions;

namespace TodoApp.Api.GraphQL.Errors;

/// <summary>
/// Turns a broken domain rule into a GraphQL error the client can show: the rule's own
/// message, a VALIDATION_ERROR code and the offending field. Without this, HotChocolate
/// hides every exception behind "Unexpected Execution Error" outside Development.
/// </summary>
public class TodoValidationErrorFilter : IErrorFilter
{
    public const string Code = "VALIDATION_ERROR";

    public IError OnError(IError error)
    {
        if (error.Exception is not TodoValidationException validation)
            return error;

        return error
            .WithMessage(validation.Message)
            .WithCode(Code)
            .SetExtension("field", validation.Field)
            .WithException(null);
    }
}

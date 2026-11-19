using HotChocolate.Execution.Configuration;
using TodoApp.Api.GraphQL.Errors;
using TodoApp.Api.GraphQL.Mutations;
using TodoApp.Api.GraphQL.Queries;
using TodoApp.Api.GraphQL.Types;

namespace TodoApp.Api.GraphQL;

public static class GraphQLSetup
{
    /// <summary>
    /// The whole GraphQL schema in one place, so the tests run against exactly what the API serves.
    /// </summary>
    public static IRequestExecutorBuilder AddTodoGraphQL(this IServiceCollection services, bool includeExceptionDetails = false) =>
        services
            .AddGraphQLServer()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddType<TodoItemType>()
            .AddType<TodoStatisticsType>()
            .AddErrorFilter<TodoValidationErrorFilter>()
            .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = includeExceptionDetails);
}

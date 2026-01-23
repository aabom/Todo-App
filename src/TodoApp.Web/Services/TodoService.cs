using TodoApp.Web.Interfaces;
using TodoApp.Web.Models.Todo;

namespace TodoApp.Web.Services;

/// <summary>
/// Application-level service for todo operations.
/// </summary>
public class TodoService : ITodoService
{
    private readonly IGraphQLService _graphQL;

    public TodoService(IGraphQLService graphQL)
    {
        _graphQL = graphQL;
    }

    public async Task<List<TodoItemModel>> GetAllAsync()
    {
        var query = """
            query {
                todos {
                    id
                    title
                    description
                    isCompleted
                    priority
                    createdAt
                    completedAt
                    dueDate
                    isOverdue
                }
            }
            """;

        var response = await _graphQL.ExecuteQueryAsync<TodosResponse>(query);
        return response?.Todos ?? new List<TodoItemModel>();
    }

    public async Task<TodoItemModel?> GetByIdAsync(Guid id)
    {
        var query = """
            query($id: UUID!) {
                todo(id: $id) {
                    id
                    title
                    description
                    isCompleted
                    priority
                    createdAt
                    completedAt
                    dueDate
                    isOverdue
                }
            }
            """;

        var response = await _graphQL.ExecuteQueryAsync<TodoResponse>(query, new { id });
        return response?.Todo;
    }

    public async Task<TodoStatisticsModel> GetStatisticsAsync()
    {
        var query = """
            query {
                statistics {
                    totalCount
                    completedCount
                    pendingCount
                    overdueCount
                }
            }
            """;

        var response = await _graphQL.ExecuteQueryAsync<StatisticsResponse>(query);
        return response?.Statistics ?? new TodoStatisticsModel();
    }

    public async Task<TodoItemModel?> CreateAsync(CreateTodoModel model)
    {
        var mutation = """
            mutation($input: CreateTodoInput!) {
                createTodo(input: $input) {
                    id
                    title
                    description
                    isCompleted
                    priority
                    createdAt
                    completedAt
                    dueDate
                    isOverdue
                }
            }
            """;

        var response = await _graphQL.ExecuteMutationAsync<CreateTodoResponse>(mutation, new
        {
            input = new
            {
                title = model.Title,
                description = model.Description,
                priority = model.Priority.ToString().ToUpperInvariant(),
                dueDate = model.DueDate
            }
        });

        return response?.CreateTodo;
    }

    public async Task<TodoItemModel?> ToggleCompleteAsync(Guid id)
    {
        var mutation = """
            mutation($id: UUID!) {
                toggleTodoComplete(id: $id) {
                    id
                    title
                    description
                    isCompleted
                    priority
                    createdAt
                    completedAt
                    dueDate
                    isOverdue
                }
            }
            """;

        var response = await _graphQL.ExecuteMutationAsync<ToggleCompleteResponse>(mutation, new { id });
        return response?.ToggleTodoComplete;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var mutation = """
            mutation($id: UUID!) {
                deleteTodo(id: $id) {
                    success
                    deletedId
                }
            }
            """;

        var response = await _graphQL.ExecuteMutationAsync<DeleteTodoResponse>(mutation, new { id });
        return response?.DeleteTodo?.Success ?? false;
    }

    public async Task<TodoItemModel?> UpdateAsync(UpdateTodoModel model)
    {
        var mutation = """
            mutation($input: UpdateTodoInput!) {
                updateTodo(input: $input) {
                    id
                    title
                    description
                    isCompleted
                    priority
                    createdAt
                    completedAt
                    dueDate
                    isOverdue
                }
            }
            """;

        var response = await _graphQL.ExecuteMutationAsync<UpdateTodoResponse>(mutation, new
        {
            input = new
            {
                id = model.Id,
                title = model.Title,
                description = model.Description,
                priority = model.Priority?.ToString().ToUpperInvariant(),
                dueDate = model.DueDate
            }
        });

        return response?.UpdateTodo;
    }
}

// Response wrapper classes
public class TodosResponse { public List<TodoItemModel> Todos { get; set; } = new(); }
public class TodoResponse { public TodoItemModel? Todo { get; set; } }
public class StatisticsResponse { public TodoStatisticsModel Statistics { get; set; } = new(); }
public class CreateTodoResponse { public TodoItemModel? CreateTodo { get; set; } }
public class UpdateTodoResponse { public TodoItemModel? UpdateTodo { get; set; } }
public class ToggleCompleteResponse { public TodoItemModel? ToggleTodoComplete { get; set; } }
public class DeleteTodoResponse { public DeletePayload? DeleteTodo { get; set; } }
public class DeletePayload { public bool Success { get; set; } public Guid DeletedId { get; set; } }

using Microsoft.AspNetCore.Components;
using TodoApp.Domain.Models;
using TodoApp.Web.Interfaces;
using TodoApp.Web.Models.Todo;

namespace TodoApp.Web.Pages;

public partial class Home
{
    [Inject] private ITodoService TodoService { get; set; } = default!;

    private List<TodoItemModel> todos = new();
    private TodoStatisticsModel? statistics;
    private CreateTodoModel newTodo = new();
    private DateTime? dueDateInput;
    private bool isLoading = false;
    private bool isInitializing = true;
    private string? errorMessage;
    private Filter currentFilter = Filter.All;

    // Edit mode state
    private Guid? editingTodoId = null;
    private EditTodoModel editModel = new();
    private DateTime? editDueDateInput;

    private enum Filter { All, Pending, Completed }

    private IEnumerable<TodoItemModel> filteredTodos => currentFilter switch
    {
        Filter.Pending => todos.Where(t => !t.IsCompleted),
        Filter.Completed => todos.Where(t => t.IsCompleted),
        _ => todos
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
        isInitializing = false;
    }

    private async Task LoadData()
    {
        try
        {
            var todosTask = TodoService.GetAllAsync();
            var statsTask = TodoService.GetStatisticsAsync();

            await Task.WhenAll(todosTask, statsTask);

            todos = await todosTask;
            statistics = await statsTask;
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to load data: {ex.Message}";
        }
    }

    private async Task AddTodo()
    {
        if (string.IsNullOrWhiteSpace(newTodo.Title)) return;

        isLoading = true;
        try
        {
            newTodo.DueDate = dueDateInput?.ToUniversalTime();
            var created = await TodoService.CreateAsync(newTodo);
            if (created != null)
            {
                todos.Insert(0, created);
                newTodo = new CreateTodoModel();
                dueDateInput = null;
                await RefreshStatistics();
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to create todo: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task ToggleComplete(Guid id)
    {
        try
        {
            var updated = await TodoService.ToggleCompleteAsync(id);
            if (updated != null)
            {
                var index = todos.FindIndex(t => t.Id == id);
                if (index >= 0)
                {
                    todos[index] = updated;
                }
                await RefreshStatistics();
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to update todo: {ex.Message}";
        }
    }

    private async Task DeleteTodo(Guid id)
    {
        try
        {
            var success = await TodoService.DeleteAsync(id);
            if (success)
            {
                todos.RemoveAll(t => t.Id == id);
                await RefreshStatistics();
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to delete todo: {ex.Message}";
        }
    }

    private void StartEdit(TodoItemModel todo)
    {
        editingTodoId = todo.Id;
        editModel = new EditTodoModel
        {
            Title = todo.Title,
            Description = todo.Description,
            Priority = todo.Priority
        };
        editDueDateInput = todo.DueDate?.ToLocalTime();
    }

    private async Task SaveEdit()
    {
        if (string.IsNullOrWhiteSpace(editModel.Title) || !editingTodoId.HasValue) return;

        try
        {
            var updateModel = new UpdateTodoModel
            {
                Id = editingTodoId.Value,
                Title = editModel.Title,
                Description = editModel.Description,
                Priority = editModel.Priority,
                DueDate = editDueDateInput?.ToUniversalTime()
            };

            var updated = await TodoService.UpdateAsync(updateModel);
            if (updated != null)
            {
                var index = todos.FindIndex(t => t.Id == editingTodoId.Value);
                if (index >= 0)
                {
                    todos[index] = updated;
                }
                editingTodoId = null;
                await RefreshStatistics();
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to update todo: {ex.Message}";
        }
    }

    private void CancelEdit()
    {
        editingTodoId = null;
        editModel = new EditTodoModel();
        editDueDateInput = null;
    }

    private async Task RefreshStatistics()
    {
        try
        {
            statistics = await TodoService.GetStatisticsAsync();
        }
        catch { /* Ignore stats refresh errors */ }
    }

    private void SetFilter(Filter filter)
    {
        currentFilter = filter;
    }

    private string GetEmptyMessage() => currentFilter switch
    {
        Filter.Pending => "No pending todos. Great job!",
        Filter.Completed => "No completed todos yet.",
        _ => "No todos yet. Add one above!"
    };

    private class EditTodoModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;
    }
}

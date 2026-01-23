using TodoApp.Domain.Models;

namespace TodoApp.Web.Models.Todo;

public class CreateTodoModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime? DueDate { get; set; }
}

using TodoApp.Domain.Models;

namespace TodoApp.Web.Models.Todo;

public class UpdateTodoModel
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Priority? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public bool ClearDueDate { get; set; }
}

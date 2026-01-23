namespace TodoApp.Web.Models.Todo;

public class TodoStatisticsModel
{
    public int TotalCount { get; set; }
    public int CompletedCount { get; set; }
    public int PendingCount { get; set; }
    public int OverdueCount { get; set; }
}

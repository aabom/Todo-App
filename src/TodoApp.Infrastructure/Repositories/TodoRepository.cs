using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Interfaces;
using TodoApp.Domain.Models;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation using EF Core.
/// </summary>
public class TodoRepository(TodoDbContext context) : ITodoRepository
{
    private readonly TodoDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<TodoItem?> GetByIdAsync(Guid id) =>
        await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IReadOnlyList<TodoItem>> GetAllAsync() =>
        await InDisplayOrder(_context.TodoItems).ToListAsync();

    public async Task<IReadOnlyList<TodoItem>> GetByStatusAsync(bool isCompleted) =>
        await InDisplayOrder(_context.TodoItems.Where(t => t.IsCompleted == isCompleted)).ToListAsync();

    public async Task<IReadOnlyList<TodoItem>> GetOverdueAsync(DateTime now) =>
        await Overdue(now).OrderBy(t => t.DueDate).ToListAsync();

    public async Task<int> GetOverdueCountAsync(DateTime now) => await Overdue(now).CountAsync();

    public async Task<TodoItem> AddAsync(TodoItem item)
    {
        await _context.TodoItems.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateAsync(TodoItem item)
    {
        _context.TodoItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await GetByIdAsync(id);
        if (item is not null)
        {
            _context.TodoItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetCountAsync(bool? isCompleted = null)
    {
        var query = _context.TodoItems.AsQueryable();

        if (isCompleted.HasValue)
            query = query.Where(t => t.IsCompleted == isCompleted.Value);

        return await query.CountAsync();
    }

    private IQueryable<TodoItem> Overdue(DateTime now) =>
        _context.TodoItems.Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate < now);

    // SQLite sorts NULL first in ascending order, which would put items without a
    // due date above urgent ones. Sorting on "has no due date" first sends them last.
    private static IQueryable<TodoItem> InDisplayOrder(IQueryable<TodoItem> query) => query
        .OrderByDescending(t => t.Priority)
        .ThenBy(t => t.DueDate == null)
        .ThenBy(t => t.DueDate)
        .ThenByDescending(t => t.CreatedAt);
}

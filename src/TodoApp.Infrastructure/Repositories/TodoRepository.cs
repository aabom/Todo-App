using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Models;
using TodoApp.Domain.Interfaces;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation using EF Core.
/// </summary>
public class TodoRepository : ITodoRepository
{
    private readonly TodoDbContext _context;

    public TodoRepository(TodoDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id)
    {
        return await _context.TodoItems
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IReadOnlyList<TodoItem>> GetAllAsync()
    {
        return await _context.TodoItems
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TodoItem>> GetByStatusAsync(bool isCompleted)
    {
        return await _context.TodoItems
            .Where(t => t.IsCompleted == isCompleted)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TodoItem>> GetOverdueAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.TodoItems
            .Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate < now)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

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
}

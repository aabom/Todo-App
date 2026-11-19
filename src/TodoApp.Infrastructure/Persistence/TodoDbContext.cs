using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TodoApp.Domain.Models;

namespace TodoApp.Infrastructure.Persistence;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(t => t.Id);
            
            entity.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(TodoItem.TitleMaxLength);
            
            entity.Property(t => t.Description)
                .HasMaxLength(TodoItem.DescriptionMaxLength);
            
            entity.Property(t => t.Priority)
                .HasConversion<int>();

            // SQLite does not store timezone info, so DateTimes are read back with
            // DateTimeKind.Unspecified. These converters ensure all DateTimes are
            // marked as UTC on read, so downstream serialization (e.g. GraphQL)
            // correctly includes the 'Z' suffix and clients can convert to local time.
            var utcConverter = new ValueConverter<DateTime, DateTime>(
                v => v,
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableUtcConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

            entity.Property(t => t.CreatedAt)
                .IsRequired()
                .HasConversion(utcConverter);

            entity.Property(t => t.CompletedAt)
                .HasConversion(nullableUtcConverter);

            entity.Property(t => t.DueDate)
                .HasConversion(nullableUtcConverter);

            // Index for common queries
            entity.HasIndex(t => t.IsCompleted);
            entity.HasIndex(t => t.DueDate);
            entity.HasIndex(t => t.Priority);
        });
    }
}

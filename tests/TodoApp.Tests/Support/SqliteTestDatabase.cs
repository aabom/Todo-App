using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Tests.Support;

/// <summary>
/// A real SQLite database held in memory for the lifetime of one test. Using SQLite
/// rather than the EF in-memory provider means queries are translated to SQL exactly as
/// in the app, so ordering and NULL handling are tested for real.
/// </summary>
public sealed class SqliteTestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteTestDatabase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    /// <summary>The open connection. An in-memory database lives exactly as long as it does.</summary>
    public SqliteConnection Connection => _connection;

    public DbContextOptions<TodoDbContext> Options =>
        new DbContextOptionsBuilder<TodoDbContext>().UseSqlite(_connection).Options;

    /// <summary>A fresh context, so reads come from the database rather than EF's change tracker.</summary>
    public TodoDbContext CreateContext() => new(Options);

    public void Dispose() => _connection.Dispose();
}

using TodoApp.Api.GraphQL;
using TodoApp.Application;
using TodoApp.Infrastructure;
using TodoApp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services using Clean Architecture DI extensions
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=todo.db";

builder.Services
    .AddApplication()
    .AddInfrastructure(connectionString);

builder.Services.AddTodoGraphQL(includeExceptionDetails: builder.Environment.IsDevelopment());

// CORS for the Blazor WebAssembly client. Only the origins listed in configuration may call the API.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Create the SQLite database on first run (see README: why EnsureCreated rather than migrations)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    db.Database.EnsureCreated();

    // Off by default; the Docker setup turns it on so a fresh volume shows sample todos.
    if (builder.Configuration.GetValue<bool>("Database:SeedDemoData"))
    {
        var now = scope.ServiceProvider.GetRequiredService<TimeProvider>().GetUtcNow().UtcDateTime;
        await DemoData.SeedIfEmptyAsync(db, now);
    }
}

app.UseCors();

app.MapGraphQL();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();

using Microsoft.EntityFrameworkCore;
using TodoApp.Api.GraphQL.Mutations;
using TodoApp.Api.GraphQL.Queries;
using TodoApp.Api.GraphQL.Types;
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

// Configure GraphQL with HotChocolate
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<TodoItemType>()
    .AddType<TodoStatisticsType>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = builder.Environment.IsDevelopment());

// Configure CORS for Blazor WebAssembly
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();

// Map GraphQL endpoint
app.MapGraphQL();

// Add a simple health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();

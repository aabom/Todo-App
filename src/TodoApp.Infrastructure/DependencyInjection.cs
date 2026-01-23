using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Domain.Interfaces;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Repositories;

namespace TodoApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<TodoDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<ITodoRepository, TodoRepository>();

        return services;
    }
}

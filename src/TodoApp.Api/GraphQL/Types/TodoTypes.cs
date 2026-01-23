using TodoApp.Application.DTOs;
using TodoApp.Domain.Models;

namespace TodoApp.Api.GraphQL.Types;

/// <summary>
/// GraphQL type for TodoItem.
/// HotChocolate automatically maps properties, but explicit types give more control.
/// </summary>
public class TodoItemType : ObjectType<TodoItemDto>
{
    protected override void Configure(IObjectTypeDescriptor<TodoItemDto> descriptor)
    {
        descriptor.Name("TodoItem");
        
        descriptor.Field(t => t.Id)
            .Type<NonNullType<UuidType>>()
            .Description("Unique identifier");
        
        descriptor.Field(t => t.Title)
            .Type<NonNullType<StringType>>()
            .Description("Todo item title");
        
        descriptor.Field(t => t.Description)
            .Type<StringType>()
            .Description("Optional detailed description");
        
        descriptor.Field(t => t.IsCompleted)
            .Type<NonNullType<BooleanType>>()
            .Description("Whether the item is completed");
        
        descriptor.Field(t => t.Priority)
            .Type<NonNullType<EnumType<Priority>>>()
            .Description("Priority level");
        
        descriptor.Field(t => t.CreatedAt)
            .Type<NonNullType<DateTimeType>>()
            .Description("Creation timestamp");
        
        descriptor.Field(t => t.CompletedAt)
            .Type<DateTimeType>()
            .Description("Completion timestamp");
        
        descriptor.Field(t => t.DueDate)
            .Type<DateTimeType>()
            .Description("Optional due date");
        
        descriptor.Field(t => t.IsOverdue)
            .Type<NonNullType<BooleanType>>()
            .Description("Whether the item is past its due date");
    }
}

public class TodoStatisticsType : ObjectType<TodoStatisticsDto>
{
    protected override void Configure(IObjectTypeDescriptor<TodoStatisticsDto> descriptor)
    {
        descriptor.Name("TodoStatistics");
        
        descriptor.Field(s => s.TotalCount)
            .Description("Total number of todos");
        
        descriptor.Field(s => s.CompletedCount)
            .Description("Number of completed todos");
        
        descriptor.Field(s => s.PendingCount)
            .Description("Number of pending todos");
        
        descriptor.Field(s => s.OverdueCount)
            .Description("Number of overdue todos");
    }
}

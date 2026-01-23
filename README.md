# Todo App - Clean Architecture with GraphQL

A modern Todo application demonstrating **Clean Architecture** principles with a **.NET 9** backend using **GraphQL** (HotChocolate) and a **Blazor WebAssembly** frontend.

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        Presentation Layer                        │
│  ┌─────────────────────────┐    ┌─────────────────────────────┐ │
│  │    TodoApp.Api          │    │      TodoApp.Web            │ │
│  │  (GraphQL Server)       │    │   (Blazor WebAssembly)      │ │
│  │  - HotChocolate         │◄───│   - GraphQL Client          │ │
│  │  - Queries/Mutations    │    │   - Razor Components        │ │
│  └───────────┬─────────────┘    └─────────────────────────────┘ │
└──────────────┼──────────────────────────────────────────────────┘
               │
┌──────────────▼──────────────────────────────────────────────────┐
│                       Application Layer                          │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │                   TodoApp.Application                        ││
│  │  - ITodoService (Interface)                                  ││
│  │  - TodoService (Implementation)                              ││
│  │  - DTOs (Data Transfer Objects)                              ││
│  │  - Use Cases / Business Logic Orchestration                  ││
│  └──────────────────────────┬──────────────────────────────────┘│
└─────────────────────────────┼───────────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────────┐
│                         Domain Layer                             │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │                    TodoApp.Domain                            ││
│  │  - TodoItem Entity (with encapsulated business rules)        ││
│  │  - Priority Enum                                             ││
│  │  - ITodoRepository (Interface - Dependency Inversion)        ││
│  └─────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
                              ▲
┌─────────────────────────────┼───────────────────────────────────┐
│                    Infrastructure Layer                          │
│  ┌─────────────────────────┴───────────────────────────────────┐│
│  │                TodoApp.Infrastructure                        ││
│  │  - TodoDbContext (EF Core)                                   ││
│  │  - TodoRepository (implements ITodoRepository)               ││
│  │  - SQLite Persistence                                        ││
│  └─────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
```

## 📁 Project Structure

```
TodoApp/
├── TodoApp.sln
└── src/
    ├── TodoApp.Domain/           # Core business entities & interfaces
    │   ├── Models/
    │   │   ├── TodoItem.cs       # Rich domain model with behavior
    │   │   └── Priority.cs       # Value object (enum)
    │   └── Interfaces/
    │       └── ITodoRepository.cs
    │
    ├── TodoApp.Application/      # Use cases & business logic
    │   ├── DTOs/
    │   │   └── TodoDtos.cs       # Data transfer objects
    │   ├── Interfaces/
    │   │   └── ITodoService.cs
    │   ├── Services/
    │   │   └── TodoService.cs    # Business logic orchestration
    │   └── DependencyInjection.cs
    │
    ├── TodoApp.Infrastructure/   # External concerns (DB, etc.)
    │   ├── Persistence/
    │   │   └── TodoDbContext.cs
    │   ├── Repositories/
    │   │   └── TodoRepository.cs
    │   └── DependencyInjection.cs
    │
    ├── TodoApp.Api/              # GraphQL API (presentation)
    │   ├── GraphQL/
    │   │   ├── Queries/
    │   │   │   └── Query.cs
    │   │   ├── Mutations/
    │   │   │   └── Mutation.cs
    │   │   └── Types/
    │   │       └── TodoTypes.cs
    │   └── Program.cs
    │
    └── TodoApp.Web/              # Blazor WebAssembly (presentation)
        ├── Pages/
        │   ├── Home.razor        # UI markup
        │   └── Home.razor.cs     # Code-behind
        ├── Models/
        │   ├── Todo/
        │   │   ├── TodoItemModel.cs
        │   │   ├── CreateTodoModel.cs
        │   │   ├── UpdateTodoModel.cs
        │   │   └── TodoStatisticsModel.cs
        │   └── GraphQL/
        │       ├── GraphQLRequest.cs
        │       ├── GraphQLResponse.cs
        │       ├── GraphQLError.cs
        │       └── GraphQLException.cs
        ├── Interfaces/
        │   ├── IGraphQLService.cs
        │   └── ITodoService.cs
        ├── Services/
        │   ├── GraphQLService.cs
        │   └── TodoService.cs
        ├── Layout/
        │   └── MainLayout.razor
        └── wwwroot/
```

## 🚀 Getting Started

### Prerequisites

- .NET 9 SDK
- Any IDE (VS Code, Visual Studio, Rider)

### Running the Application

1. **Start the API** (in one terminal):
   ```bash
   cd src/TodoApp.Api
   dotnet run
   ```
   The GraphQL API will be available at `http://localhost:5000/graphql`
   
   Access the GraphQL IDE (Banana Cake Pop) at the same URL.

2. **Start the Blazor Frontend** (in another terminal):
   ```bash
   cd src/TodoApp.Web
   dotnet run
   ```
   The web app will be available at `http://localhost:5001`

### Using the GraphQL API

Open `http://localhost:5000/graphql` in your browser to access Banana Cake Pop (GraphQL IDE).

**Example Queries:**

```graphql
# Get all todos
query {
  todos {
    id
    title
    description
    isCompleted
    priority
    createdAt
    dueDate
    isOverdue
  }
}

# Get statistics
query {
  statistics {
    totalCount
    completedCount
    pendingCount
    overdueCount
  }
}

# Create a todo
mutation {
  createTodo(input: {
    title: "Learn GraphQL"
    description: "Study HotChocolate documentation"
    priority: HIGH
    dueDate: "2025-12-31T14:00:00"
  }) {
    id
    title
    priority
  }
}

# Update a todo
mutation {
  updateTodo(input: {
    id: "your-todo-id-here"
    title: "Learn GraphQL (updated)"
    priority: CRITICAL
  }) {
    id
    title
    priority
  }
}

# Update a todo with variable
mutation($id: UUID!) {
  updateTodo(input: {
    id: $id
    title: "Learn GraphQL (updated)"
    priority: CRITICAL
  }) {
    id
    title
    priority
  }
}

# Toggle completion
mutation {
  toggleTodoComplete(id: "your-todo-id-here") {
    id
    isCompleted
    completedAt
  }
}
```

## 🔧 Technical Stack

| Component | Technology |
|-----------|------------|
| Backend Framework | .NET 9 |
| GraphQL Server | HotChocolate 13.9 |
| ORM | Entity Framework Core 9 |
| Database | SQLite |
| Frontend | Blazor WebAssembly |
| GraphQL Client | Custom lightweight client |

# Todo App: Clean Architecture with GraphQL

[![CI](https://github.com/aabom/Todo-App/actions/workflows/ci.yml/badge.svg)](https://github.com/aabom/Todo-App/actions/workflows/ci.yml)

A todo application built as a small but complete example of **Clean Architecture** in .NET 10.
A **GraphQL** API (HotChocolate) stores todos in **SQLite** through EF Core. A
**Blazor WebAssembly** front end talks to that API through a hand-written GraphQL client.

![Screenshot of the app](docs/screenshot.png)

## What it does

- Create, edit, complete and delete todos, each with a title, an optional description, a priority
  and an optional due date and time.
- The list is sorted by priority, then by the earliest due date (todos without one go last), then
  by the newest first.
- Todos past their due date are flagged as overdue. The header shows totals for all, pending,
  completed and overdue.
- The list can be filtered to all, pending or completed todos.

## Running it

**Prerequisite:** the [.NET 10 SDK](https://dotnet.microsoft.com/download). The version is
pinned in `global.json`.

```bash
# Terminal 1: the API, at http://localhost:5000/graphql
cd src/TodoApp.Api
dotnet run

# Terminal 2: the front end, at http://localhost:5001
cd src/TodoApp.Web
dotnet run
```

The API creates `todo.db` on first start. Opening `http://localhost:5000/graphql` in a browser
loads Nitro, HotChocolate's GraphQL IDE, where the schema can be explored and queried.

## Running the tests

```bash
dotnet test
```

The tests cover every layer:

| Area | What is tested | How |
|------|----------------|-----|
| Domain | Title and description rules, completion, overdue logic | Plain unit tests |
| Infrastructure | Sort order, overdue queries, UTC round-trip of dates | Real SQLite, in memory |
| Application | Partial updates, clearing fields, statistics against the clock | `FakeTimeProvider` + SQLite |
| API | The GraphQL schema the server exposes, including error shape | HotChocolate request executor |
| Web | The Blazor page: ordering after adding, what an edit sends | bUnit |

CI runs the build and the tests on every push and pull request. Warnings are errors, and a
package with a known vulnerability fails the build.

## Architecture

```
TodoApp.Web (Blazor WASM) ──GraphQL over HTTP──► TodoApp.Api (HotChocolate)
                                                        │
                                                        ▼
                                              TodoApp.Application
                                           (use cases, DTOs, TodoService)
                                                        │
                                                        ▼
                                                 TodoApp.Domain
                                     (TodoItem, Priority, ITodoRepository)
                                                        ▲
                                                        │ implements
                                              TodoApp.Infrastructure
                                         (EF Core, SQLite, TodoRepository)
```

```
src/
├── TodoApp.Domain/          Entity with its own rules, repository interface. No dependencies.
├── TodoApp.Application/     Use cases (TodoService) and DTOs.
├── TodoApp.Infrastructure/  EF Core DbContext and repository on SQLite.
├── TodoApp.Api/             GraphQL queries, mutations, types and error mapping.
└── TodoApp.Web/             Blazor WebAssembly UI and a small GraphQL client.
tests/
└── TodoApp.Tests/           One test project, one folder per layer.
```

## Design decisions

**A rich domain model.** `TodoItem` has private setters, and every change goes through a method.
The rules therefore live in one place and cannot be bypassed: a title is required and trimmed,
and the title and description have maximum lengths. A broken rule throws
`TodoValidationException`.

**Domain errors become readable GraphQL errors.** An error filter turns a
`TodoValidationException` into a GraphQL error carrying the rule's own message, the code
`VALIDATION_ERROR` and the offending field. Without the filter, the client would only ever see
"Unexpected Execution Error".

**Partial updates distinguish "unchanged" from "remove".** In `updateTodo`, a field that is left
out stays as it is. An empty `description` removes the description. A due date is removed with
`clearDueDate: true`. The flag is explicit because a missing `dueDate` and `dueDate: null` look
the same once they reach C#. Sending both a due date and `clearDueDate` is rejected.

**Time is injected.** The domain never reads the clock. `TodoService` gets the time from
`TimeProvider` and passes it in, so overdue logic is deterministic and the tests control the
clock.

**All times are UTC.** The client converts its local input to UTC before sending it. SQLite
stores no time zone, so value converters in `TodoDbContext` mark dates as UTC when they are
read back. The UTC marker is what makes GraphQL serialize them with a `Z` suffix, which in turn
lets the browser show them in local time.

**The server owns the sort order.** After adding or editing a todo, the page reloads the list
rather than guessing where the item belongs. The sort is applied in SQL. Items without a due date
are sorted last explicitly, because SQLite puts `NULL` first.

**A hand-written GraphQL client.** The front end sends the queries as plain strings through a
small `GraphQLService` rather than using a generated client. With five operations, the code
stays easy to read and has no build-time code generation.

**SQLite and `EnsureCreated`.** The database is a single file, so the app runs with no setup.
Because this is a demo with one table and no deployed data to preserve, the schema is created
with `EnsureCreated` rather than migrations. A real deployment would switch to migrations.

**The UI reuses the domain's `Priority` enum.** The Web project references the Domain project
for that single enum, so the two cannot drift apart. Nothing else crosses that boundary.

**CORS is limited to the front end's origin.** It is configured in `Cors:AllowedOrigins` in the
API's `appsettings.json`.

## Example operations

```graphql
query {
  todos { id title priority dueDate isOverdue }
  statistics { totalCount completedCount pendingCount overdueCount }
}

mutation {
  createTodo(input: { title: "Learn GraphQL", priority: HIGH, dueDate: "2030-12-31T14:00:00Z" }) {
    id
  }
}

mutation($id: UUID!) {
  updateTodo(input: { id: $id, priority: CRITICAL, clearDueDate: true }) {
    id priority dueDate
  }
}

mutation($id: UUID!) {
  toggleTodoComplete(id: $id) { isCompleted completedAt }
}
```

## Tech stack

| Component | Technology |
|-----------|------------|
| Runtime | .NET 10 |
| GraphQL server | HotChocolate 15 |
| ORM and database | EF Core 10, SQLite |
| Front end | Blazor WebAssembly |
| Tests | xUnit v3, bUnit, Microsoft.Extensions.TimeProvider.Testing |
| CI | GitHub Actions |

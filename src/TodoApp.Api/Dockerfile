# Build from the repository root:  docker build -f src/TodoApp.Api/Dockerfile .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# Project files first, so the restore layer is cached until a dependency changes.
COPY global.json Directory.Build.props ./
COPY src/TodoApp.Domain/TodoApp.Domain.csproj src/TodoApp.Domain/
COPY src/TodoApp.Application/TodoApp.Application.csproj src/TodoApp.Application/
COPY src/TodoApp.Infrastructure/TodoApp.Infrastructure.csproj src/TodoApp.Infrastructure/
COPY src/TodoApp.Api/TodoApp.Api.csproj src/TodoApp.Api/
RUN dotnet restore src/TodoApp.Api/TodoApp.Api.csproj

COPY src/TodoApp.Domain/ src/TodoApp.Domain/
COPY src/TodoApp.Application/ src/TodoApp.Application/
COPY src/TodoApp.Infrastructure/ src/TodoApp.Infrastructure/
COPY src/TodoApp.Api/ src/TodoApp.Api/
RUN dotnet publish src/TodoApp.Api/TodoApp.Api.csproj --no-restore -c Release -o /app

# The runtime image has no shell, so the data directory is made here and copied across,
# owned by the image's non-root user (UID 1654).
RUN mkdir /data

# Chiseled: the ASP.NET runtime and nothing else. No shell, no package manager, runs as non-root.
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app
COPY --from=build --chown=1654:1654 /data /data
COPY --from=build /app .

ENV ASPNETCORE_HTTP_PORTS=8080 \
    ConnectionStrings__DefaultConnection="Data Source=/data/todo.db"
VOLUME /data
EXPOSE 8080
ENTRYPOINT ["dotnet", "TodoApp.Api.dll"]

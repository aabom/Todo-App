import { useState } from 'react'
import type { TodoApi, UpdateTodoInput } from './api/types'
import { AddTodoForm } from './components/AddTodoForm'
import { FilterTabs } from './components/FilterTabs'
import { StatsPanel } from './components/StatsPanel'
import { TodoItem } from './components/TodoItem'
import { useTodos } from './hooks/useTodos'
import { applyFilter, type Filter } from './lib/filters'

const EMPTY_MESSAGES: Record<Filter, string> = {
  all: 'No todos yet. Add one above!',
  pending: 'No pending todos. Great job!',
  completed: 'No completed todos yet.',
}

interface AppProps {
  api: TodoApi
}

export function App({ api }: AppProps) {
  const { todos, statistics, isLoading, error, dismissError, create, update, toggle, remove } = useTodos(api)
  const [filter, setFilter] = useState<Filter>('all')
  const [editingId, setEditingId] = useState<string | null>(null)

  const visible = applyFilter(todos, filter)

  const saveEdit = async (input: UpdateTodoInput) => {
    if (await update(input)) setEditingId(null)
  }

  return (
    <div className="app-container">
      <header className="app-header">
        <div className="header-content">
          <h1>Todo App</h1>
          <p className="subtitle">React client</p>
        </div>
      </header>

      <main className="main-content">
        <div className="todo-app">
          {statistics && <StatsPanel statistics={statistics} />}

          <AddTodoForm onAdd={create} />

          <FilterTabs todos={todos} current={filter} onChange={setFilter} />

          {isLoading ? (
            <div className="loading">
              <div className="spinner" />
              <p>Loading todos...</p>
            </div>
          ) : visible.length === 0 ? (
            <div className="empty-state">
              <p>{EMPTY_MESSAGES[filter]}</p>
            </div>
          ) : (
            <ul className="todo-list" aria-label="Todos">
              {visible.map((todo) => (
                <TodoItem
                  key={todo.id}
                  todo={todo}
                  isEditing={editingId === todo.id}
                  onToggle={(id) => void toggle(id)}
                  onDelete={(id) => void remove(id)}
                  onStartEdit={setEditingId}
                  onCancelEdit={() => {
                    setEditingId(null)
                  }}
                  onSave={saveEdit}
                />
              ))}
            </ul>
          )}

          {error && (
            <div className="error-toast" role="alert">
              <span>{error}</span>
              <button type="button" aria-label="Dismiss" onClick={dismissError}>
                ×
              </button>
            </div>
          )}
        </div>
      </main>

      <footer className="app-footer">
        <p>Built with React 19 • TypeScript • Vite • HotChocolate GraphQL • .NET 10</p>
      </footer>
    </div>
  )
}

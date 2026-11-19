import type { Todo, UpdateTodoInput } from '../api/types'
import { formatLocal } from '../lib/dates'
import { priorityLabel } from '../lib/priority'
import { EditTodoForm } from './EditTodoForm'

interface TodoItemProps {
  todo: Todo
  isEditing: boolean
  onToggle: (id: string) => void
  onDelete: (id: string) => void
  onStartEdit: (id: string) => void
  onCancelEdit: () => void
  onSave: (input: UpdateTodoInput) => Promise<void>
}

export function TodoItem({ todo, isEditing, onToggle, onDelete, onStartEdit, onCancelEdit, onSave }: TodoItemProps) {
  const classes = [
    'todo-item',
    todo.isCompleted && 'completed',
    todo.isOverdue && 'overdue',
    `priority-${todo.priority.toLowerCase()}`,
  ]
    .filter(Boolean)
    .join(' ')

  if (isEditing) {
    return (
      <li className={classes}>
        <EditTodoForm todo={todo} onSave={onSave} onCancel={onCancelEdit} />
      </li>
    )
  }

  return (
    <li className={classes}>
      <div className="todo-checkbox">
        <input
          type="checkbox"
          aria-label={`Mark ${todo.title} as ${todo.isCompleted ? 'not completed' : 'completed'}`}
          checked={todo.isCompleted}
          onChange={() => {
            onToggle(todo.id)
          }}
        />
      </div>
      <div className="todo-content">
        <div className="todo-header">
          <span className="todo-title">{todo.title}</span>
          <span className={`priority-badge ${todo.priority.toLowerCase()}`}>{priorityLabel(todo.priority)}</span>
        </div>
        {todo.description && <p className="todo-description">{todo.description}</p>}
        <div className="todo-meta">
          <span className="created-date">Created: {formatLocal(todo.createdAt)}</span>
          {todo.dueDate && (
            <span className={`due-date ${todo.isOverdue ? 'overdue' : ''}`.trim()}>
              Due: {formatLocal(todo.dueDate)}
              {todo.isOverdue && <span className="overdue-badge">OVERDUE</span>}
            </span>
          )}
          {todo.completedAt && <span className="completed-date">Completed: {formatLocal(todo.completedAt)}</span>}
        </div>
      </div>
      <div className="todo-actions">
        <button
          type="button"
          className="edit-btn"
          aria-label={`Edit ${todo.title}`}
          onClick={() => {
            onStartEdit(todo.id)
          }}
        >
          ✏️
        </button>
        <button
          type="button"
          className="delete-btn"
          aria-label={`Delete ${todo.title}`}
          onClick={() => {
            onDelete(todo.id)
          }}
        >
          🗑️
        </button>
      </div>
    </li>
  )
}

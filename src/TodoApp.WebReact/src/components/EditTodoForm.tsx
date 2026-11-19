import { useState, type SyntheticEvent } from 'react'
import type { Priority, Todo, UpdateTodoInput } from '../api/types'
import { toLocalInputValue } from '../lib/dates'
import { buildUpdate } from '../lib/updateInput'
import { DueDateInput } from './DueDateInput'
import { PrioritySelect } from './PrioritySelect'

interface EditTodoFormProps {
  todo: Todo
  onSave: (input: UpdateTodoInput) => Promise<void>
  onCancel: () => void
}

export function EditTodoForm({ todo, onSave, onCancel }: EditTodoFormProps) {
  const [title, setTitle] = useState(todo.title)
  const [description, setDescription] = useState(todo.description ?? '')
  const [priority, setPriority] = useState<Priority>(todo.priority)
  const [dueDate, setDueDate] = useState(toLocalInputValue(todo.dueDate))

  const handleSubmit = async (e: SyntheticEvent) => {
    e.preventDefault()
    if (!title.trim()) return
    await onSave(buildUpdate(todo.id, title, description, priority, dueDate))
  }

  return (
    <form className="todo-edit-form" aria-label={`Edit ${todo.title}`} onSubmit={(e) => void handleSubmit(e)}>
      <div className="form-row">
        <input
          type="text"
          className="title-input"
          aria-label="Title"
          placeholder="Title"
          value={title}
          onChange={(e) => {
            setTitle(e.target.value)
          }}
        />
      </div>
      <div className="form-row">
        <input
          type="text"
          className="description-input"
          aria-label="Description"
          placeholder="Description (optional)"
          value={description}
          onChange={(e) => {
            setDescription(e.target.value)
          }}
        />
      </div>
      <div className="form-row inline">
        <PrioritySelect value={priority} onChange={setPriority} />
        <DueDateInput value={dueDate} onChange={setDueDate} />
        <div className="edit-actions">
          <button type="submit" className="save-btn" disabled={!title.trim()}>
            Save
          </button>
          <button type="button" className="cancel-btn" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </div>
    </form>
  )
}

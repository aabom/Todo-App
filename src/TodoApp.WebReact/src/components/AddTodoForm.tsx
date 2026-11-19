import { useState, type SyntheticEvent } from 'react'
import type { CreateTodoInput, Priority } from '../api/types'
import { fromLocalInputValue } from '../lib/dates'
import { DueDateInput } from './DueDateInput'
import { PrioritySelect } from './PrioritySelect'

interface AddTodoFormProps {
  /** Resolves true when the todo was created, which clears the form. */
  onAdd: (input: CreateTodoInput) => Promise<boolean>
}

export function AddTodoForm({ onAdd }: AddTodoFormProps) {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [priority, setPriority] = useState<Priority>('MEDIUM')
  const [dueDate, setDueDate] = useState('')
  const [isSaving, setIsSaving] = useState(false)

  const handleSubmit = async (e: SyntheticEvent) => {
    e.preventDefault()
    if (!title.trim()) return

    setIsSaving(true)
    const created = await onAdd({
      title,
      description: description || null,
      priority,
      dueDate: fromLocalInputValue(dueDate),
    })
    setIsSaving(false)

    if (created) {
      setTitle('')
      setDescription('')
      setPriority('MEDIUM')
      setDueDate('')
    }
  }

  return (
    <form className="add-todo-form" aria-label="Add new todo" onSubmit={(e) => void handleSubmit(e)}>
      <h2>Add New Todo</h2>
      <div className="form-row">
        <input
          type="text"
          className="title-input"
          aria-label="Title"
          placeholder="What needs to be done?"
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
        <button type="submit" className="add-btn" disabled={!title.trim() || isSaving}>
          {isSaving ? 'Adding...' : '+ Add Todo'}
        </button>
      </div>
    </form>
  )
}

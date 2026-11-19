import { useId } from 'react'
import { PRIORITIES, type Priority } from '../api/types'
import { isPriority, priorityLabel } from '../lib/priority'

interface PrioritySelectProps {
  value: Priority
  onChange: (priority: Priority) => void
}

export function PrioritySelect({ value, onChange }: PrioritySelectProps) {
  const id = useId()
  return (
    <div className="form-group">
      <label htmlFor={id}>Priority</label>
      <select
        id={id}
        value={value}
        onChange={(e) => {
          if (isPriority(e.target.value)) onChange(e.target.value)
        }}
      >
        {PRIORITIES.map((p) => (
          <option key={p} value={p}>
            {priorityLabel(p)}
          </option>
        ))}
      </select>
    </div>
  )
}

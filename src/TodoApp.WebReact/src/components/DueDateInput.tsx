import { useId } from 'react'

interface DueDateInputProps {
  /** A `datetime-local` value in local time, or "" for none. */
  value: string
  onChange: (value: string) => void
}

export function DueDateInput({ value, onChange }: DueDateInputProps) {
  const id = useId()
  return (
    <div className="form-group">
      <label htmlFor={id}>Due date &amp; time</label>
      <input
        id={id}
        type="datetime-local"
        value={value}
        onChange={(e) => {
          onChange(e.target.value)
        }}
      />
    </div>
  )
}

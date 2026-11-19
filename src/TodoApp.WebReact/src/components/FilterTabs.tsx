import type { Todo } from '../api/types'
import { applyFilter, type Filter } from '../lib/filters'

const TABS: { filter: Filter; label: string }[] = [
  { filter: 'all', label: 'All' },
  { filter: 'pending', label: 'Pending' },
  { filter: 'completed', label: 'Completed' },
]

interface FilterTabsProps {
  todos: readonly Todo[]
  current: Filter
  onChange: (filter: Filter) => void
}

export function FilterTabs({ todos, current, onChange }: FilterTabsProps) {
  return (
    <div className="filter-tabs" role="group" aria-label="Filter">
      {TABS.map(({ filter, label }) => (
        <button
          key={filter}
          type="button"
          className={current === filter ? 'active' : ''}
          aria-pressed={current === filter}
          onClick={() => {
            onChange(filter)
          }}
        >
          {label} ({applyFilter(todos, filter).length})
        </button>
      ))}
    </div>
  )
}

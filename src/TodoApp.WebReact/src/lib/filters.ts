import type { Todo } from '../api/types'

export type Filter = 'all' | 'pending' | 'completed'

export function applyFilter(todos: readonly Todo[], filter: Filter): Todo[] {
  switch (filter) {
    case 'pending':
      return todos.filter((t) => !t.isCompleted)
    case 'completed':
      return todos.filter((t) => t.isCompleted)
    case 'all':
      return [...todos]
  }
}

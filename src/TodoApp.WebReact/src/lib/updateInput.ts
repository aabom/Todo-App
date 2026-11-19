import type { Priority, UpdateTodoInput } from '../api/types'
import { fromLocalInputValue } from './dates'

/**
 * The edit form shows every field, so what it sends is the whole new state: an emptied
 * description is sent as "" and an emptied due date as `clearDueDate: true`, both of
 * which the API reads as "remove it".
 */
export function buildUpdate(
  id: string,
  title: string,
  description: string,
  priority: Priority,
  dueDate: string,
): UpdateTodoInput {
  const dueDateUtc = fromLocalInputValue(dueDate)
  return dueDateUtc
    ? { id, title, description, priority, dueDate: dueDateUtc }
    : { id, title, description, priority, clearDueDate: true }
}

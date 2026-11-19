import { PRIORITIES, type Priority } from '../api/types'

const LABELS: Record<Priority, string> = {
  LOW: 'Low',
  MEDIUM: 'Medium',
  HIGH: 'High',
  CRITICAL: 'Critical',
}

export const priorityLabel = (priority: Priority) => LABELS[priority]

export const isPriority = (value: string): value is Priority => (PRIORITIES as readonly string[]).includes(value)

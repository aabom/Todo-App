import { useCallback, useEffect, useState } from 'react'
import type { CreateTodoInput, Todo, TodoApi, TodoStatistics, UpdateTodoInput } from '../api/types'

const messageOf = (error: unknown) => (error instanceof Error ? error.message : String(error))

export interface TodosState {
  todos: Todo[]
  statistics: TodoStatistics | null
  isLoading: boolean
  error: string | null
  dismissError: () => void
  create: (input: CreateTodoInput) => Promise<boolean>
  update: (input: UpdateTodoInput) => Promise<boolean>
  toggle: (id: string) => Promise<void>
  remove: (id: string) => Promise<void>
}

/**
 * The page's data and the operations on it. The server owns the sort order, so after a
 * create or an edit (which can change priority or due date) the list is reloaded rather
 * than patched. A toggle or delete cannot move other items, so those patch in place.
 */
export function useTodos(api: TodoApi): TodosState {
  const [todos, setTodos] = useState<Todo[]>([])
  const [statistics, setStatistics] = useState<TodoStatistics | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const reload = useCallback(async () => {
    const [nextTodos, nextStatistics] = await Promise.all([api.getTodos(), api.getStatistics()])
    setTodos(nextTodos)
    setStatistics(nextStatistics)
  }, [api])

  // The change itself has already succeeded when this runs, so a failure is reported on
  // its own rather than as a failed update. The previous figures stay on screen.
  const refreshStatistics = useCallback(async () => {
    try {
      setStatistics(await api.getStatistics())
    } catch (e) {
      setError(`Your change was saved, but the totals could not be refreshed: ${messageOf(e)}`)
    }
  }, [api])

  // Initial load. State is only set once the requests settle, and not at all if the
  // component has unmounted by then.
  useEffect(() => {
    let cancelled = false
    Promise.all([api.getTodos(), api.getStatistics()])
      .then(([initialTodos, initialStatistics]) => {
        if (cancelled) return
        setTodos(initialTodos)
        setStatistics(initialStatistics)
      })
      .catch((e: unknown) => {
        if (!cancelled) setError(`Failed to load data: ${messageOf(e)}`)
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [api])

  const create = useCallback(
    async (input: CreateTodoInput) => {
      try {
        await api.createTodo(input)
        await reload()
        return true
      } catch (e) {
        setError(`Failed to create todo: ${messageOf(e)}`)
        return false
      }
    },
    [api, reload],
  )

  const update = useCallback(
    async (input: UpdateTodoInput) => {
      try {
        const updated = await api.updateTodo(input)
        if (!updated) return false
        await reload()
        return true
      } catch (e) {
        setError(`Failed to update todo: ${messageOf(e)}`)
        return false
      }
    },
    [api, reload],
  )

  const toggle = useCallback(
    async (id: string) => {
      try {
        const updated = await api.toggleTodo(id)
        if (!updated) return
        setTodos((current) => current.map((t) => (t.id === id ? updated : t)))
      } catch (e) {
        setError(`Failed to update todo: ${messageOf(e)}`)
        return
      }
      await refreshStatistics()
    },
    [api, refreshStatistics],
  )

  const remove = useCallback(
    async (id: string) => {
      try {
        if (!(await api.deleteTodo(id))) return
        setTodos((current) => current.filter((t) => t.id !== id))
      } catch (e) {
        setError(`Failed to delete todo: ${messageOf(e)}`)
        return
      }
      await refreshStatistics()
    },
    [api, refreshStatistics],
  )

  const dismissError = useCallback(() => {
    setError(null)
  }, [])

  return { todos, statistics, isLoading, error, dismissError, create, update, toggle, remove }
}

import { render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { App } from './App'
import { GraphQLRequestError } from './api/graphqlClient'
import { FakeTodoApi, makeTodo } from './test/fakeTodoApi'

async function renderApp(api: FakeTodoApi) {
  const user = userEvent.setup()
  render(<App api={api} />)
  await screen.findByRole('region', { name: 'Statistics' })
  return user
}

const titles = () =>
  within(screen.getByRole('list', { name: 'Todos' }))
    .getAllByRole('listitem')
    .map((li) => li.querySelector('.todo-title')?.textContent)

describe('App', () => {
  it('lists todos in server order with statistics', async () => {
    const api = new FakeTodoApi().seed(
      makeTodo({ title: 'Someday', priority: 'LOW' }),
      makeTodo({ title: 'Urgent', priority: 'CRITICAL' }),
    )
    await renderApp(api)

    expect(titles()).toEqual(['Urgent', 'Someday'])
    const stats = screen.getByRole('region', { name: 'Statistics' })
    expect(within(stats).getByText('Total').previousSibling).toHaveTextContent('2')
  })

  it('places an added todo where the server sorts it', async () => {
    const api = new FakeTodoApi().seed(makeTodo({ title: 'Urgent', priority: 'CRITICAL' }))
    const user = await renderApp(api)
    const form = screen.getByRole('form', { name: 'Add new todo' })

    await user.type(within(form).getByLabelText('Title'), 'Someday')
    await user.selectOptions(within(form).getByLabelText('Priority'), 'LOW')
    await user.click(within(form).getByRole('button', { name: '+ Add Todo' }))

    await waitFor(() => {
      expect(titles()).toEqual(['Urgent', 'Someday'])
    })
    expect(within(form).getByLabelText('Title')).toHaveValue('')
  })

  it('sends a local due date to the API as UTC', async () => {
    const api = new FakeTodoApi()
    const user = await renderApp(api)
    const form = screen.getByRole('form', { name: 'Add new todo' })

    await user.type(within(form).getByLabelText('Title'), 'Dentist')
    // Tests run in Europe/Stockholm, which is UTC+1 in December.
    await user.type(within(form).getByLabelText('Due date & time'), '2026-12-02T10:00')
    await user.click(within(form).getByRole('button', { name: '+ Add Todo' }))

    await waitFor(() => {
      expect(api.creates).toHaveLength(1)
    })
    expect(api.creates[0]?.dueDate).toBe('2026-12-02T09:00:00.000Z')
    expect(await screen.findByText('Due: 2026-12-02 10:00')).toBeInTheDocument()
  })

  it('asks the server to clear the due date and description when they are emptied', async () => {
    const api = new FakeTodoApi().seed(
      makeTodo({ title: 'Task', description: 'notes', dueDate: '2030-01-01T00:00:00.000Z' }),
    )
    const user = await renderApp(api)

    await user.click(screen.getByRole('button', { name: 'Edit Task' }))
    const form = screen.getByRole('form', { name: 'Edit Task' })
    await user.clear(within(form).getByLabelText('Due date & time'))
    await user.clear(within(form).getByLabelText('Description'))
    await user.click(within(form).getByRole('button', { name: 'Save' }))

    await waitFor(() => {
      expect(api.updates).toHaveLength(1)
    })
    expect(api.updates[0]).toEqual({
      id: expect.any(String) as string,
      title: 'Task',
      description: '',
      priority: 'MEDIUM',
      clearDueDate: true,
    })
    await waitFor(() => {
      expect(screen.queryByRole('form', { name: 'Edit Task' })).not.toBeInTheDocument()
    })
    expect(screen.queryByText(/^Due:/)).not.toBeInTheDocument()
  })

  it('keeps the due date when editing leaves it alone', async () => {
    const api = new FakeTodoApi().seed(makeTodo({ title: 'Task', dueDate: '2030-01-01T00:00:00.000Z' }))
    const user = await renderApp(api)

    await user.click(screen.getByRole('button', { name: 'Edit Task' }))
    await user.click(screen.getByRole('button', { name: 'Save' }))

    await waitFor(() => {
      expect(api.updates).toHaveLength(1)
    })
    expect(api.updates[0]).toMatchObject({ dueDate: '2030-01-01T00:00:00.000Z' })
    expect(api.updates[0]).not.toHaveProperty('clearDueDate')
  })

  it('reports a failed statistics refresh without undoing the change', async () => {
    const api = new FakeTodoApi().seed(makeTodo({ title: 'Task' }))
    const user = await renderApp(api)
    api.statisticsFail = true

    await user.click(screen.getByRole('checkbox', { name: 'Mark Task as completed' }))

    expect(await screen.findByRole('alert')).toHaveTextContent('totals could not be refreshed')
    expect(screen.getByRole('checkbox', { name: 'Mark Task as not completed' })).toBeChecked()
  })

  it("shows the API's validation message", async () => {
    const api = new FakeTodoApi()
    api.createError = new GraphQLRequestError([
      { message: 'Title cannot be longer than 200 characters.', extensions: { code: 'VALIDATION_ERROR', field: 'Title' } },
    ])
    const user = await renderApp(api)
    const form = screen.getByRole('form', { name: 'Add new todo' })

    await user.type(within(form).getByLabelText('Title'), 'Too long')
    await user.click(within(form).getByRole('button', { name: '+ Add Todo' }))

    expect(await screen.findByRole('alert')).toHaveTextContent('Title cannot be longer than 200 characters.')
    expect(within(form).getByLabelText('Title')).toHaveValue('Too long')
  })

  it('filters by status and deletes', async () => {
    const api = new FakeTodoApi().seed(
      makeTodo({ title: 'Open' }),
      makeTodo({ title: 'Done', isCompleted: true, completedAt: '2026-01-15T13:00:00.000Z' }),
    )
    const user = await renderApp(api)

    await user.click(screen.getByRole('button', { name: 'Completed (1)' }))
    expect(titles()).toEqual(['Done'])

    await user.click(screen.getByRole('button', { name: 'Pending (1)' }))
    expect(titles()).toEqual(['Open'])

    await user.click(screen.getByRole('button', { name: 'Delete Open' }))
    expect(await screen.findByText('No pending todos. Great job!')).toBeInTheDocument()
  })
})

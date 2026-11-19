import { expect, test, type Page } from '@playwright/test'

// The database is shared and seeded, so every test makes its own todos with unique titles
// and never asserts on totals.
const unique = (label: string) =>
  `${label} ${Date.now().toString(36)}${Math.random().toString(36).slice(2, 6)}`

const todoList = (page: Page) => page.getByRole('list', { name: 'Todos' })
const todoItem = (page: Page, title: string) =>
  todoList(page).getByRole('listitem').filter({ hasText: title })

async function addTodo(page: Page, title: string, options: { priority?: string; due?: string } = {}) {
  const form = page.getByRole('form', { name: 'Add new todo' })
  await form.getByLabel('Title').fill(title)
  if (options.priority) await form.getByLabel('Priority').selectOption({ label: options.priority })
  if (options.due) await form.getByLabel('Due date & time').fill(options.due)
  await form.getByRole('button', { name: 'Add Todo' }).click()
  await expect(todoItem(page, title)).toBeVisible()
}

async function statistic(page: Page, label: string): Promise<number> {
  const text = await page.getByRole('region', { name: 'Statistics' }).innerText()
  const match = new RegExp(`(\\d+)\\s*${label}`, 'i').exec(text)
  if (!match?.[1]) throw new Error(`No "${label}" in statistics: ${text}`)
  return Number(match[1])
}

test.beforeEach(async ({ page }) => {
  await page.goto('/')
  await expect(todoList(page)).toBeVisible()
})

test('a new todo is saved on the server', async ({ page }) => {
  const title = unique('Persisted')
  await addTodo(page, title)

  await page.reload()
  await expect(todoItem(page, title)).toBeVisible()
})

test('completing a todo moves it between filters and updates the statistics', async ({ page }) => {
  const title = unique('Complete me')
  await addTodo(page, title)
  const completedBefore = await statistic(page, 'completed')

  // The box only changes once the server has answered, so click and wait for the new state
  // rather than check(), which expects the change to be immediate.
  await page.getByRole('checkbox', { name: `Mark ${title} as completed` }).click()

  await expect(page.getByRole('checkbox', { name: `Mark ${title} as not completed` })).toBeChecked()
  await expect.poll(() => statistic(page, 'completed')).toBe(completedBefore + 1)

  const filters = page.getByRole('group', { name: 'Filter' })
  await filters.getByRole('button', { name: /^Pending/ }).click()
  await expect(todoItem(page, title)).toHaveCount(0)
  await filters.getByRole('button', { name: /^Completed/ }).click()
  await expect(todoItem(page, title)).toBeVisible()
})

test('the server places a todo by priority and due date', async ({ page }) => {
  const urgent = unique('Urgent')
  const someday = unique('Someday')
  await addTodo(page, someday, { priority: 'Low' })
  await addTodo(page, urgent, { priority: 'Critical', due: '2020-01-01T09:00' })

  const titles = await todoList(page).locator('.todo-title').allInnerTexts()
  expect(titles.indexOf(urgent)).toBeGreaterThanOrEqual(0)
  expect(titles.indexOf(urgent)).toBeLessThan(titles.indexOf(someday))
})

test('a due date round-trips through UTC and shows as overdue in local time', async ({ page }) => {
  const title = unique('Overdue')
  await addTodo(page, title, { due: '2020-01-01T09:00' })

  const item = todoItem(page, title)
  await expect(item).toContainText('Due: 2020-01-01 09:00')
  await expect(item).toContainText('OVERDUE')
})

test('editing a todo can rename it and clear its due date', async ({ page }) => {
  const title = unique('Rename me')
  const renamed = unique('Renamed')
  await addTodo(page, title, { due: '2030-06-15T14:30' })

  await page.getByRole('button', { name: `Edit ${title}` }).click()
  const form = page.getByRole('form', { name: `Edit ${title}` })
  await form.getByLabel('Title').fill(renamed)
  await form.getByLabel('Due date & time').fill('')
  await form.getByRole('button', { name: 'Save' }).click()
  await expect(todoItem(page, renamed)).toBeVisible()

  await page.reload()
  const item = todoItem(page, renamed)
  await expect(item).toBeVisible()
  await expect(item).not.toContainText('Due:')
  await expect(todoItem(page, title)).toHaveCount(0)
})

test('a todo can be deleted', async ({ page }) => {
  const title = unique('Delete me')
  await addTodo(page, title)

  await page.getByRole('button', { name: `Delete ${title}` }).click()
  await expect(todoItem(page, title)).toHaveCount(0)

  await page.reload()
  await expect(todoList(page)).toBeVisible()
  await expect(todoItem(page, title)).toHaveCount(0)
})

test('a validation error from the API is shown to the user', async ({ page }) => {
  const form = page.getByRole('form', { name: 'Add new todo' })
  await form.getByLabel('Title').fill('x'.repeat(201))
  await form.getByRole('button', { name: 'Add Todo' }).click()

  await expect(page.getByRole('alert')).toContainText('Title cannot be longer than 200 characters')
})

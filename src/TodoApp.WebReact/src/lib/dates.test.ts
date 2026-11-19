import { formatLocal, fromLocalInputValue, toLocalInputValue } from './dates'

// Tests run in Europe/Stockholm (see vite.config.ts): UTC+1 in winter, UTC+2 in summer.
describe('dates', () => {
  it('shows UTC times in local time', () => {
    expect(formatLocal('2026-01-15T12:00:00.000Z')).toBe('2026-01-15 13:00')
    expect(formatLocal('2026-07-15T12:00:00.000Z')).toBe('2026-07-15 14:00')
  })

  it('converts a local input value to UTC', () => {
    expect(fromLocalInputValue('2026-01-15T13:00')).toBe('2026-01-15T12:00:00.000Z')
  })

  it('round-trips through the date input', () => {
    const utc = '2026-12-02T09:30:00.000Z'
    expect(fromLocalInputValue(toLocalInputValue(utc))).toBe(utc)
  })

  it('treats empty and invalid input as no date', () => {
    expect(fromLocalInputValue('')).toBeNull()
    expect(fromLocalInputValue('not a date')).toBeNull()
    expect(toLocalInputValue(null)).toBe('')
  })
})

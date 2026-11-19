// The API speaks UTC; the user thinks in local time. These are the only places the two meet.

const pad = (n: number) => String(n).padStart(2, '0')

/** "2026-11-21T09:00:00.000Z" → "yyyy-MM-dd HH:mm" in the browser's time zone. */
export function formatLocal(utcIso: string): string {
  const d = new Date(utcIso)
  return `${String(d.getFullYear())}-${pad(d.getMonth() + 1)}-${pad(d.getDate())} ${pad(d.getHours())}:${pad(d.getMinutes())}`
}

/** UTC ISO string → the value a `<input type="datetime-local">` expects, in local time. */
export function toLocalInputValue(utcIso: string | null): string {
  if (!utcIso) return ''
  return formatLocal(utcIso).replace(' ', 'T')
}

/** A `datetime-local` value (local time) → a UTC ISO string, or null when empty or invalid. */
export function fromLocalInputValue(value: string): string | null {
  if (!value) return null
  // Without a zone suffix, the Date constructor reads "yyyy-MM-ddTHH:mm" as local time.
  const d = new Date(value)
  return Number.isNaN(d.getTime()) ? null : d.toISOString()
}

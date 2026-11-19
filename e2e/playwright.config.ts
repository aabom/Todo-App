import { defineConfig, devices } from '@playwright/test'

// These tests drive the app as it runs in Docker: start it first with `docker compose up --build`.
// BASE_URL points them at another running copy.
export default defineConfig({
  testDir: './tests',
  // The tests share one database, so they run one at a time and only assert on todos they create.
  fullyParallel: false,
  workers: 1,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  reporter: process.env.CI ? [['list'], ['html', { open: 'never' }]] : 'list',
  use: {
    baseURL: process.env.BASE_URL ?? 'http://localhost:8080',
    // A zone with an offset from UTC, so the tests prove due dates are converted both ways.
    timezoneId: 'Europe/Stockholm',
    trace: 'retain-on-failure',
  },
  projects: [{ name: 'chromium', use: { ...devices['Desktop Chrome'] } }],
})

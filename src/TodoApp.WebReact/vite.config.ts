import react from '@vitejs/plugin-react'
import { defineConfig } from 'vitest/config'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    strictPort: true,
    // The client calls /graphql on its own origin; in development that is forwarded to the
    // API, the same way nginx forwards it in the Docker image. No CORS involved.
    proxy: {
      '/graphql': process.env.API_URL ?? 'http://localhost:5000',
    },
  },
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: ['./src/test/setup.ts'],
    restoreMocks: true,
    // A zone with an offset from UTC, so the tests prove times are converted.
    env: { TZ: 'Europe/Stockholm' },
  },
})

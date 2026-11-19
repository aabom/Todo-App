import react from '@vitejs/plugin-react'
import { defineConfig } from 'vitest/config'

export default defineConfig({
  plugins: [react()],
  server: {
    // Fixed so the API's CORS policy (Cors:AllowedOrigins) can name it.
    port: 5173,
    strictPort: true,
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

import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { createGraphQLClient } from './api/graphqlClient'
import { createTodoApi } from './api/todoApi'
import { App } from './App'
import './index.css'

// By default the client calls /graphql on its own origin, and whatever serves it routes that
// to the API: the Vite dev server's proxy locally, nginx in the Docker image. VITE_API_URL
// (read at build time) points it at an API on another origin instead, for static hosts that
// cannot proxy.
const apiBaseUrl = import.meta.env.VITE_API_URL ?? ''
const api = createTodoApi(createGraphQLClient(`${apiBaseUrl}/graphql`))

const root = document.getElementById('root')
if (!root) throw new Error('Missing #root element')

createRoot(root).render(
  <StrictMode>
    <App api={api} />
  </StrictMode>,
)

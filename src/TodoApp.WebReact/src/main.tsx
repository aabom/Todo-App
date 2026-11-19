import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { createGraphQLClient } from './api/graphqlClient'
import { createTodoApi } from './api/todoApi'
import { App } from './App'
import './index.css'

const apiBaseUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5000'
const api = createTodoApi(createGraphQLClient(`${apiBaseUrl}/graphql`))

const root = document.getElementById('root')
if (!root) throw new Error('Missing #root element')

createRoot(root).render(
  <StrictMode>
    <App api={api} />
  </StrictMode>,
)

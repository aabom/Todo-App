import { createGraphQLClient, GraphQLRequestError, HttpRequestError } from './graphqlClient'

const respond = (status: number, body: unknown) =>
  vi.fn<typeof fetch>().mockResolvedValue(
    new Response(JSON.stringify(body), { status, headers: { 'Content-Type': 'application/json' } }),
  )

describe('createGraphQLClient', () => {
  it('posts the document and variables and returns data', async () => {
    const fetchMock = respond(200, { data: { todos: [] } })
    const execute = createGraphQLClient('http://api/graphql', fetchMock)

    const data = await execute<{ todos: unknown[] }>('query { todos { id } }', { a: 1 })

    expect(data).toEqual({ todos: [] })
    const [url, init] = fetchMock.mock.calls[0] ?? []
    expect(url).toBe('http://api/graphql')
    expect(init?.method).toBe('POST')
    expect(JSON.parse(init?.body as string)).toEqual({ query: 'query { todos { id } }', variables: { a: 1 } })
  })

  it('turns a VALIDATION_ERROR into a readable error', async () => {
    const execute = createGraphQLClient(
      'http://api/graphql',
      respond(200, {
        data: null,
        errors: [{ message: 'Title cannot be empty.', extensions: { code: 'VALIDATION_ERROR', field: 'Title' } }],
      }),
    )

    const error = await execute('mutation { x }').catch((e: unknown) => e)

    expect(error).toBeInstanceOf(GraphQLRequestError)
    const gqlError = error as GraphQLRequestError
    expect(gqlError.message).toBe('Title cannot be empty.')
    expect(gqlError.isValidationError).toBe(true)
    expect(gqlError.invalidFields).toEqual(['Title'])
  })

  it('reads GraphQL errors even when the status is not 2xx', async () => {
    const execute = createGraphQLClient('http://api/graphql', respond(400, { errors: [{ message: 'Syntax error' }] }))

    const error = await execute('{').catch((e: unknown) => e)

    expect(error).toBeInstanceOf(GraphQLRequestError)
    expect((error as GraphQLRequestError).isValidationError).toBe(false)
  })

  it('fails with the HTTP status when there is no GraphQL body', async () => {
    const fetchMock = vi.fn<typeof fetch>().mockResolvedValue(new Response('down', { status: 503, statusText: 'Service Unavailable' }))
    const execute = createGraphQLClient('http://api/graphql', fetchMock)

    const error = await execute('{ todos { id } }').catch((e: unknown) => e)

    expect(error).toBeInstanceOf(HttpRequestError)
    expect((error as HttpRequestError).status).toBe(503)
  })
})

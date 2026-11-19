/** One entry of a GraphQL response's `errors` array. */
export interface GraphQLErrorEntry {
  message: string
  extensions?: {
    code?: string
    field?: string
  }
}

interface GraphQLResponse<TData> {
  data?: TData | null
  errors?: GraphQLErrorEntry[]
}

/** The code the API puts on a broken domain rule (see TodoValidationErrorFilter). */
export const VALIDATION_ERROR = 'VALIDATION_ERROR'

/**
 * Thrown when the API answers with GraphQL errors. For a validation error the message
 * is the rule's own text ("Title cannot be empty."), ready to show to the user.
 */
export class GraphQLRequestError extends Error {
  readonly errors: readonly GraphQLErrorEntry[]

  constructor(errors: readonly GraphQLErrorEntry[]) {
    super(errors.map((e) => e.message).join('; '))
    this.name = 'GraphQLRequestError'
    this.errors = errors
  }

  /** True when every error is a broken domain rule rather than a server fault. */
  get isValidationError(): boolean {
    return this.errors.length > 0 && this.errors.every((e) => e.extensions?.code === VALIDATION_ERROR)
  }

  /** The fields named by validation errors, e.g. ["Title"]. */
  get invalidFields(): string[] {
    return this.errors.flatMap((e) => (e.extensions?.field ? [e.extensions.field] : []))
  }
}

/** Thrown when the request does not get a usable GraphQL response at all. */
export class HttpRequestError extends Error {
  readonly status: number

  constructor(status: number, statusText: string) {
    super(`The API answered ${String(status)} ${statusText}`.trim())
    this.name = 'HttpRequestError'
    this.status = status
  }
}

/**
 * A document that carries the types of its result and variables, as the documents generated
 * into `src/gql` do. At runtime it is just the query text.
 */
export interface TypedDocument<TResult, TVariables> {
  readonly __apiType?: (variables: TVariables) => TResult
  toString(): string
}

export type GraphQLExecutor = <TResult, TVariables>(
  document: TypedDocument<TResult, TVariables>,
  variables?: TVariables,
) => Promise<TResult>

/**
 * A minimal typed GraphQL client: one POST per operation, `data` returned typed by the document,
 * `errors` thrown. Queries and mutations travel the same way, so one function serves both.
 */
export function createGraphQLClient(endpoint: string, fetchImpl: typeof fetch = fetch): GraphQLExecutor {
  return async <TResult, TVariables>(
    document: TypedDocument<TResult, TVariables>,
    variables?: TVariables,
  ): Promise<TResult> => {
    const response = await fetchImpl(endpoint, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
      body: JSON.stringify({ query: document.toString(), variables }),
    })

    // HotChocolate answers GraphQL errors with a 4xx/5xx status but still sends a
    // GraphQL body, so read the body before deciding the request failed.
    const body = (await response.json().catch(() => null)) as GraphQLResponse<TResult> | null

    if (body?.errors?.length) {
      throw new GraphQLRequestError(body.errors)
    }
    if (!response.ok || body?.data == null) {
      throw new HttpRequestError(response.status, response.statusText)
    }
    return body.data
  }
}

import type { CodegenConfig } from '@graphql-codegen/cli'

// The TypeScript types come from the schema the API serves. That file is kept current by a
// .NET test (SchemaFileTests), and CI fails if `npm run codegen` would change anything.
const config: CodegenConfig = {
  schema: '../TodoApp.Api/schema.graphql',
  documents: ['src/**/*.ts', '!src/gql/**/*'],
  ignoreNoDocuments: false,
  generates: {
    './src/gql/': {
      preset: 'client',
      presetConfig: {
        // Plain strings, sent as-is by the fetch-based client in api/graphqlClient.ts.
        fragmentMasking: false,
      },
      config: {
        documentMode: 'string',
        enumsAsTypes: true,
        useTypeImports: true,
        strictScalars: true,
        scalars: { UUID: 'string', DateTime: 'string' },
      },
    },
  },
}

export default config

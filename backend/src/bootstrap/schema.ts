import { join } from 'node:path'
import { GraphQLFileLoader } from '@graphql-tools/graphql-file-loader'
import { loadSchema } from '@graphql-tools/load'

// Load schema from the file
export const schema = await loadSchema(join(__dirname, '../graphql/schema.graphql'), {
  loaders: [new GraphQLFileLoader()]
});
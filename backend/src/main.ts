import { createServer } from 'http'
import { join } from 'node:path'
import { createYoga } from 'graphql-yoga'
import { GraphQLFileLoader } from '@graphql-tools/graphql-file-loader'
import { loadSchema } from '@graphql-tools/load'
import { addResolversToSchema } from '@graphql-tools/schema'
import { resolvers } from './resolvers'

async function main() {
    // Load schema from the file
    const schema = await loadSchema(join(__dirname, './graphql/schema.graphql'), {
      loaders: [new GraphQLFileLoader()]
    })
      
    // Add resolvers to the schema
    const schemaWithResolvers = addResolversToSchema({ schema, resolvers })
   
    const yoga = createYoga({
      schema: schemaWithResolvers
    })
   
    const server = createServer(yoga)
   
    server.listen(4000, () => {
      console.log('Server is running on http://localhost:4000')
    })
  }
   
  main().catch(error => {
    console.error(error)
    process.exit(1)
  })
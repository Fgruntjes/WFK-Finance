import { createYoga } from 'graphql-yoga'
import { addResolversToSchema } from '@graphql-tools/schema'
import { resolvers } from './resolvers.js'
import { schema } from './schema.js';


export const yoga = createYoga({
    schema: addResolversToSchema({ schema, resolvers })
});
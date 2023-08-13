 // Add resolvers to the schema
import { yoga } from "@/bootstrap/yoga";
import { buildHTTPExecutor } from '@graphql-tools/executor-http'

export const executor = buildHTTPExecutor({
    fetch: yoga.fetch
});
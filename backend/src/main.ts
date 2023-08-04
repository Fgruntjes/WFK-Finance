import "reflect-metadata";

import { ApolloServer } from '@apollo/server';
import { startStandaloneServer } from '@apollo/server/standalone';
import { buildSchema } from "type-graphql";
import logger from "./logger";

const schema = await buildSchema({
  resolvers: [__dirname + "/**/*.resolver.{ts,js}"],
  validate: true,
});

const server = new ApolloServer({ schema });
const { url } = await startStandaloneServer(server);

logger.info(`Server ready at ${url}`);

import { institution } from "./resolvers/institution";
import { institutions } from "./resolvers/institutions";

export const resolvers = {
    Query: {
        institutions,
        institution
    },
    //Mutation: {},
    //Subscription: {},
}
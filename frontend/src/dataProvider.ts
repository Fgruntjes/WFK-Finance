import restDataProvider from "@refinedev/simple-rest";

export const dataProvider = restDataProvider(import.meta.env.APP_API_URI);
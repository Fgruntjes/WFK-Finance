import { InstitutionConnection } from "@Api";
import { httpClient as authHttpClient } from "ra-auth-auth0";
import simpleRestProvider from "ra-data-simple-rest";
import { DataProvider } from "react-admin";
import auth0Client from "./authClient";

const httpClient = authHttpClient(auth0Client);
function resourceUri(resource: string, action: string) {
  return `${import.meta.env.APP_API_URI}/${resource}/${action}`;
}

const dataProvider: DataProvider = {
  ...simpleRestProvider(import.meta.env.APP_API_URI, httpClient),
  institutionConnectionRefreshById: (
    connectionId: string,
  ): Promise<InstitutionConnection> =>
    httpClient(
      resourceUri("institutionconnections", `refresh/id/${connectionId}`),
      { method: "PUT" },
    ).then((r) => r.json),
  institutionConnectionRefreshByExternal: (
    externalId: string,
  ): Promise<InstitutionConnection> =>
    httpClient(
      resourceUri("institutionconnections", `refresh/external/${externalId}`),
      { method: "PUT" },
    ).then((r) => r.json),
};

export default dataProvider;

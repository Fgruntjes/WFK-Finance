import { Auth0Client } from "@auth0/auth0-spa-js";

const auth0Client = new Auth0Client({
  domain: import.meta.env.AUTH0_DOMAIN,
  clientId: import.meta.env.AUTH0_CLIENT_ID,
  cacheLocation: "localstorage",
  useRefreshTokens: true,
  authorizationParams: {
    audience: import.meta.env.AUTH0_AUDIENCE,
  },
});

export default auth0Client;

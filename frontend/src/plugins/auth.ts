import { createAuth0 } from "@auth0/auth0-vue";

const auth = createAuth0({
  domain: import.meta.env.AUTH0_DOMAIN,
  clientId: import.meta.env.AUTH0_CLIENT_ID,
  
  authorizationParams: {
    redirect_uri: window.location.origin,
    audience: import.meta.env.AUTH0_AUDIENCE,
    scope: `openid profile email ${import.meta.env.AUTH0_SCOPE}`,
  }
})

export default auth;
import { Auth0AuthProvider } from "ra-auth-auth0";
import auth0Client from "./authClient";

const authProvider = Auth0AuthProvider(auth0Client, {
  loginRedirectUri: import.meta.env.LOGIN_REDIRECT_URL,
  logoutRedirectUri: import.meta.env.LOGOUT_REDIRECT_URL,
});

export default authProvider;

import { useAuth0 } from "@auth0/auth0-react";
import { AuthBindings } from "@refinedev/core";
import axiosInstance from "../rest-data-provider/utils/axios";

function useAuthProvider() {
  const { isLoading, user, logout, getAccessTokenSilently, loginWithRedirect } =
    useAuth0();

  const authProvider: AuthBindings = {
    login: async () => {
      return {
        success: true,
      };
    },
    logout: async () => {
      logout({
        logoutParams: {
          returnTo: window.location.origin,
        },
      });
      return {
        success: true,
      };
    },
    onError: async (error) => {
      return { error };
    },
    check: async () => {
      try {
        const token = await getAccessTokenSilently({
          authorizationParams: {
            audience: import.meta.env.AUTH0_AUDIENCE,
          },
        });
        if (token) {
          axiosInstance.defaults.headers.common = {
            Authorization: `Bearer ${token}`,
          };
          return {
            authenticated: true,
          };
        } else {
          loginWithRedirect();
          return {
            authenticated: false,
            error: {
              message: "Check failed",
              name: "Token not found",
            },
            logout: true,
          };
        }
      } catch (error: unknown) {
        loginWithRedirect();
        return {
          authenticated: false,
          error: error as Error,
          logout: true,
        };
      }
    },
    getPermissions: async () => null,
    getIdentity: async () => {
      if (user) {
        return {
          ...user,
          avatar: user.picture,
        };
      }
      return null;
    },
  };

  return { isLoading, authProvider };
}

export default useAuthProvider;

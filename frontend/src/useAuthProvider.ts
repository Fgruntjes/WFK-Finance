import { useAuth0 } from "@auth0/auth0-react";
import { AuthBindings } from "@refinedev/core";
import { axiosInstance } from "@refinedev/simple-rest";

export function useAuthProvider()
{
    const { isLoading, user, logout, getAccessTokenSilently } = useAuth0();
    
    const authProvider: AuthBindings & { isLoading: boolean } = {
    isLoading,
    login: async () => {
        return {
        success: true,
        };
    },
    logout: async () => {
        logout({ returnTo: window.location.origin });
        return {
            success: true,
        };
    },
    onError: async (error) => {
        console.error(error);
        return { error };
    },
    check: async () => {
        try {
            const token = await getAccessTokenSilently();
            
            if (token) {
                axiosInstance.defaults.headers.common['Authorization'] = `Bearer ${token}`;
                
                return {
                    authenticated: true,
                };
            } else {
                return {
                    authenticated: false,
                    error: {
                        message: "Check failed",
                        name: "Token not found",
                    },
                    redirectTo: "/login",
                    logout: true,
                };
            }
        } catch (error: any) {
            return {
                authenticated: false,
                error: new Error(error),
                redirectTo: "/login",
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
    return authProvider;
}
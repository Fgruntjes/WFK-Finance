import { useAuth0 } from "@auth0/auth0-react";
import { AuthBindings } from "@refinedev/core";
import { axiosInstance } from "@refinedev/simple-rest";
import globalAxios from 'axios';
import useLocalStorageState from "use-local-storage-state";

type StoredToken = {
    access_token: string;
    id_token: string;
    expires_at: number; // in ms
}

const useToken = () => {
    const { getAccessTokenSilently } = useAuth0();
    const [storedValue, setStoredValue] = useLocalStorageState<StoredToken>('auth-token');

    
    return {
        clearToken: async () => {
            setStoredValue(undefined);
        },
        getToken: async (): Promise<StoredToken> => {
            
            if (storedValue && storedValue.expires_at > Date.now()) {
                return storedValue;
            }

            const newToken = await getAccessTokenSilently({detailedResponse: true});
            const newStoredValue = {
                access_token: newToken.access_token,
                id_token: newToken.id_token,
                expires_at: Math.floor(Date.now() + (newToken.expires_in * 1000)),
            };
            
            setStoredValue(newStoredValue);
            return newStoredValue;
        }
    }
}

export function useAuthProvider()
{
    const { isLoading, user, logout } = useAuth0();
    const { getToken, clearToken } = useToken();
    
    const authProvider: AuthBindings & { isLoading: boolean } = {
    isLoading,
    login: async () => {
        return {
            success: true,
        };
    },
    logout: async () => {
        clearToken();
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
            const token = await getToken();
            
            if (token) {
                axiosInstance.defaults.headers.common['Authorization'] = `Bearer ${token.access_token}`;
                globalAxios.defaults.headers.common['Authorization'] = `Bearer ${token.access_token}`;
                
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
import ErrorElement from "@/components/ErrorElement";
import Loader from "@/components/Loader";
import RedirectingLoader from "@/components/RedirectingLoader";
import NotificationLayout from "@/layouts/NotificationLayout";
import { Auth0Provider, useAuth0 } from "@auth0/auth0-react";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

type AuthWrapperProps = {
    children: React.ReactNode
}

export function AuthGuard({ children }: AuthWrapperProps) {
    const { isLoading, error, isAuthenticated, loginWithRedirect } = useAuth0();
    
    useEffect(() => {
        if (!isAuthenticated && !isLoading) {
            loginWithRedirect({
                appState: { returnTo: window.location.pathname.replace("/logout", "") },
            });
        }
    }, [isLoading, isAuthenticated, loginWithRedirect]);

    if (error) {
        return <NotificationLayout><ErrorElement error={error} /></NotificationLayout>;
    }

    if (isLoading) {
        return <NotificationLayout><Loader /></NotificationLayout>;
    }

    if (!isAuthenticated) {
        return <NotificationLayout><RedirectingLoader/></NotificationLayout>;
    }

    return <>{children}</>;
}

export function AuthProvider({ children }: AuthWrapperProps) {
    const navigate = useNavigate();
    return (
        <Auth0Provider
                    domain={import.meta.env.AUTH0_DOMAIN}
                    clientId={import.meta.env.AUTH0_CLIENT_ID}
                    authorizationParams={{
                        audience: import.meta.env.AUTH0_AUDIENCE,
                        scope: `openid profile email ${import.meta.env.AUTH0_SCOPE}`,
                        redirect_uri: window.location.origin
                    }}
                    onRedirectCallback={(state) => navigate(state?.returnTo || "/")}
                > 
            {children}
        </Auth0Provider>
    );
}

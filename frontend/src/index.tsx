import React from "react";
import { createRoot } from "react-dom/client";

import { Auth0Provider } from "@auth0/auth0-react";
import App from "./App";

import "./i18n-provider/i18n";
import LoadingView from "./pages/LoadingView";

const container = document.getElementById("root") as HTMLElement;
const root = createRoot(container);

root.render(
  <React.StrictMode>
    <React.Suspense fallback={<LoadingView />}>
      <Auth0Provider
        domain={import.meta.env.AUTH0_DOMAIN}
        clientId={import.meta.env.AUTH0_CLIENT_ID}
        authorizationParams={{
          audience: import.meta.env.AUTH0_AUDIENCE,
          redirect_uri: window.location.origin,
        }}
      >
        <App />
      </Auth0Provider>
    </React.Suspense>
  </React.StrictMode>,
);

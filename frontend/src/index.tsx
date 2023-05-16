import { Auth0Provider } from "@auth0/auth0-react";
import React from "react";
import { createRoot } from "react-dom/client";

import App from "./App";
import "./i18n";

const container = document.getElementById("root") as HTMLElement;
const root = createRoot(container);

root.render(
  <React.StrictMode>
    <React.Suspense fallback="loading">
      <Auth0Provider
        domain={`${import.meta.env.AUTH0_DOMAIN}`}
        clientId={`${import.meta.env.AUTH0_CLIENT_ID}`}
        audience={`${import.meta.env.AUTH0_AUDIENCE}`}
        scope={`${import.meta.env.AUTH0_SCOPE}`}
        redirectUri={window.location.origin}
      >
        <App />
      </Auth0Provider>
    </React.Suspense>
  </React.StrictMode>
);

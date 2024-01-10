import { Authenticated, Refine } from "@refinedev/core";
import { RefineKbar, RefineKbarProvider } from "@refinedev/kbar";

import {
  ErrorComponent,
  ThemedLayoutV2,
  useNotificationProvider,
} from "@refinedev/antd";
import "@refinedev/antd/dist/reset.css";

import {
  InstitutionConnectionsCreateReturnView,
  InstitutionConnectionsCreateView,
  InstitutionConnectionsListView,
} from "@pages";
import routerBindings, {
  CatchAllNavigate,
  DocumentTitleHandler,
  NavigateToResource,
  UnsavedChangesNotifier,
} from "@refinedev/react-router-v6";
import { App as AntdApp } from "antd";
import { BrowserRouter, Outlet, Route, Routes } from "react-router-dom";
import { ColorModeContextProvider } from "./contexts/color-mode";
import useAuthProvider from "./hooks/useAuthProvider";
import useI18nProvider from "./i18n-provider/useI18nProvider";
import LoadingView from "./pages/LoadingView";
import Login from "./pages/Login";
import dataProvider from "./rest-data-provider";

function App() {
  const { isLoading, authProvider } = useAuthProvider();
  const i18nProvider = useI18nProvider();

  if (isLoading) {
    return <LoadingView />;
  }

  return (
    <BrowserRouter>
      <RefineKbarProvider>
        <ColorModeContextProvider>
          <AntdApp>
            <Refine
              notificationProvider={useNotificationProvider}
              routerProvider={routerBindings}
              dataProvider={dataProvider(import.meta.env.APP_API_URI)}
              authProvider={authProvider}
              i18nProvider={i18nProvider}
              options={{
                syncWithLocation: true,
                warnWhenUnsavedChanges: true,
                useNewQueryKeys: true,
                disableTelemetry: true,
              }}
              resources={[
                {
                  name: "institutionconnections",
                  list: "/bank-accounts",
                  show: "/bank-accounts/:id",
                  create: "/bank-accounts/create",
                },
              ]}
            >
              <Routes>
                <Route
                  element={
                    <ThemedLayoutV2>
                      <Outlet />
                    </ThemedLayoutV2>
                  }
                >
                  <Route
                    element={
                      <Authenticated
                        key="authenticated-routes"
                        fallback={<CatchAllNavigate to="/login" />}
                      >
                        <Outlet />
                      </Authenticated>
                    }
                  >
                    <Route index element={<div>Homepage</div>} />

                    <Route path="/bank-accounts">
                      <Route
                        index
                        element={<InstitutionConnectionsListView />}
                      />
                      <Route
                        path="create"
                        element={<InstitutionConnectionsCreateView />}
                      />
                      <Route
                        path="create-return"
                        element={<InstitutionConnectionsCreateReturnView />}
                      />
                    </Route>
                  </Route>

                  <Route
                    element={
                      <Authenticated key="auth-pages" fallback={<Outlet />}>
                        <NavigateToResource resource="institutionconnections" />
                      </Authenticated>
                    }
                  >
                    <Route path="/login" element={<Login />} />
                  </Route>

                  <Route
                    element={
                      <Authenticated key="catch-all">
                        <Outlet />
                      </Authenticated>
                    }
                  >
                    <Route path="*" element={<ErrorComponent />} />
                  </Route>
                </Route>
              </Routes>
              <RefineKbar />
              <UnsavedChangesNotifier />
              <DocumentTitleHandler />
            </Refine>
          </AntdApp>
        </ColorModeContextProvider>
      </RefineKbarProvider>
    </BrowserRouter>
  );
}

export default App;

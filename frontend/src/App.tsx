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
  DocumentTitleHandler,
  UnsavedChangesNotifier,
} from "@refinedev/react-router-v6";
import { App as AntdApp } from "antd";
import { BrowserRouter, Outlet, Route, Routes } from "react-router-dom";
import { ColorModeContextProvider } from "./contexts/color-mode";
import useAuthProvider from "./hooks/useAuthProvider";
import useI18nProvider from "./i18n-provider/useI18nProvider";
import AppFooter from "./layout/AppFooter";
import LoadingView from "./pages/LoadingView";
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
                    <Authenticated key="authenticated-routes">
                      <ThemedLayoutV2 dashboard Footer={AppFooter}>
                        <Outlet />
                      </ThemedLayoutV2>
                    </Authenticated>
                  }
                >
                  <Route index element={<div>Homepage</div>} />

                  <Route path="/bank-accounts">
                    <Route index element={<InstitutionConnectionsListView />} />
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

                <Route path="*" element={<ErrorComponent />} />
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

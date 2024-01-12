import { Refine } from "@refinedev/core";
import { RefineKbar, RefineKbarProvider } from "@refinedev/kbar";

import { useNotificationProvider } from "@refinedev/antd";
import "@refinedev/antd/dist/reset.css";

import routerBindings, {
  DocumentTitleHandler,
  UnsavedChangesNotifier,
} from "@refinedev/react-router-v6";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { App as AntdApp } from "antd";
import { BrowserRouter } from "react-router-dom";
import AppRoutes from "./AppRoutes";
import { ColorModeContextProvider } from "./contexts/color-mode";
import useAuthProvider from "./hooks/useAuthProvider";
import useI18nProvider from "./i18n-provider/useI18nProvider";
import LoadingView from "./pages/LoadingView";
import { resourceList } from "./resources";
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
              resources={resourceList}
            >
              <AppRoutes />
              <RefineKbar />
              <UnsavedChangesNotifier />
              <DocumentTitleHandler />
              <ReactQueryDevtools initialIsOpen={false} />
            </Refine>
          </AntdApp>
        </ColorModeContextProvider>
      </RefineKbarProvider>
    </BrowserRouter>
  );
}

export default App;

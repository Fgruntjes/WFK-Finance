import {
  Authenticated,
  ErrorComponent,
  Refine
} from "@refinedev/core";
import { RefineKbar, RefineKbarProvider } from "@refinedev/kbar";

import { notificationProvider, RefineThemes, ThemedLayoutV2, ThemedTitleV2 } from "@refinedev/mantine";

import {
  ColorScheme,
  ColorSchemeProvider,
  Global,
  MantineProvider
} from "@mantine/core";
import { useLocalStorage } from "@mantine/hooks";
import { NotificationsProvider } from "@mantine/notifications";
import routerBindings, {
  NavigateToResource,
  UnsavedChangesNotifier,
} from "@refinedev/react-router-v6";
import { BrowserRouter, Outlet, Route, Routes } from "react-router-dom";
import { dataProvider } from './dataProvider';
import { BankConnectionCreate, BankConnectionEdit, BankConnectionList, BankConnectionShow } from "./pages/bankconnections";
import { Login } from "./pages/login";
import { resources } from "./resources";
import { useAuthProvider } from "./useAuthProvider";
import { useI18nProvider } from "./useI18nProvider";

function App() {
  const [colorScheme, setColorScheme] = useLocalStorage<ColorScheme>({
    key: "mantine-color-scheme",
    defaultValue: "light",
    getInitialValueInEffect: true,
  });
  const authProvider = useAuthProvider();
  const i18nProvider = useI18nProvider();

  const toggleColorScheme = (value?: ColorScheme) =>
    setColorScheme(value || (colorScheme === "dark" ? "light" : "dark"));

  if (authProvider.isLoading) {
    return <span>loading...</span>;
  }

  return (
    <BrowserRouter>
      <RefineKbarProvider>
        <ColorSchemeProvider
          colorScheme={colorScheme}
          toggleColorScheme={toggleColorScheme}
        >
          {/* You can change the theme colors here. example: theme={{ ...RefineThemes.Magenta, colorScheme:colorScheme }} */}
          <MantineProvider
            theme={{ ...RefineThemes.Blue, colorScheme: colorScheme }}
            withNormalizeCSS
            withGlobalStyles
          >
            <Global styles={{ body: { WebkitFontSmoothing: "auto" } }} />
            <NotificationsProvider position="top-right">
              <Refine
                dataProvider={dataProvider}
                notificationProvider={notificationProvider}
                routerProvider={routerBindings}
                authProvider={authProvider}
                i18nProvider={i18nProvider}
                options={{
                  syncWithLocation: true,
                  warnWhenUnsavedChanges: true,
                }}
                resources={resources}>
                <Routes>
                  <Route
                      element={
                          <Authenticated fallback={<Outlet />}>
                              <NavigateToResource resource="BankConnection" />
                          </Authenticated>
                      }
                  >
                      <Route
                          path="/login"
                          element={<Login />}
                      />
                  </Route>
                  <Route
                      element={
                          <Authenticated redirectOnFail="/login">
                              <ThemedLayoutV2 Title={(props) => <ThemedTitleV2 text="WFK Finance" {...props} />}>
                                  <Outlet />
                              </ThemedLayoutV2>
                          </Authenticated>
                      }
                  >
                      <Route
                          index
                          element={<NavigateToResource resource="BankConnection" />}
                      />
                      <Route path="BankConnection">
                        <Route index element={<BankConnectionList />} />
                        <Route path="create" element={<BankConnectionCreate />} />
                        <Route path="edit/:id" element={<BankConnectionEdit />} />
                        <Route path="show/:id" element={<BankConnectionShow />} />
                      </Route>
                      <Route path="*" element={<ErrorComponent />} />
                  </Route>
                </Routes>
                <RefineKbar />
                <UnsavedChangesNotifier />
              </Refine>
            </NotificationsProvider>
          </MantineProvider>
        </ColorSchemeProvider>
      </RefineKbarProvider>
    </BrowserRouter>
  );
}

export default App;

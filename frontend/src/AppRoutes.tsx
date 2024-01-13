import {
  InstitutionAccountsShowView,
  InstitutionConnectionsCreateReturnView,
  InstitutionConnectionsCreateView,
  InstitutionConnectionsListView,
} from "@pages";
import { ThemedLayoutV2 } from "@refinedev/antd";
import { Authenticated, ErrorComponent } from "@refinedev/core";
import { Outlet, Route, Routes } from "react-router-dom";
import AppFooter from "./layout/AppFooter";

function AppRoutes() {
  return (
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
          <Route path=":id" element={<InstitutionAccountsShowView />} />
          <Route path="create" element={<InstitutionConnectionsCreateView />} />
          <Route
            path="create-return"
            element={<InstitutionConnectionsCreateReturnView />}
          />
        </Route>
      </Route>

      <Route path="*" element={<ErrorComponent />} />
    </Routes>
  );
}

export default AppRoutes;

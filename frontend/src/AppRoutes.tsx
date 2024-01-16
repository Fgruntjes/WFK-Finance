import {
  InstitutionAccountsCreateReturnView,
  InstitutionAccountsCreateView,
  InstitutionAccountsListView,
  InstitutionAccountsShowView,
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
          <Route index element={<InstitutionAccountsListView />} />
          <Route path=":id" element={<InstitutionAccountsShowView />} />
          <Route path="create" element={<InstitutionAccountsCreateView />} />
          <Route
            path="create-return"
            element={<InstitutionAccountsCreateReturnView />}
          />
          <Route path="transactions" element={<div>transactions</div>} />
        </Route>
      </Route>

      <Route path="*" element={<ErrorComponent />} />
    </Routes>
  );
}

export default AppRoutes;

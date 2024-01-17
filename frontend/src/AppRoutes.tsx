import InstitutionAccountsCreateReturnView from "@pages/institutionconnections/CreateReturnView";
import InstitutionAccountsCreateView from "@pages/institutionconnections/CreateView";
import InstitutionAccountsListView from "@pages/institutionconnections/ListView";
import InstitutionAccountsShowView from "@pages/institutionconnections/ShowView";
import InstitutionTransactionsListView from "@pages/institutiontransactions/ListView";
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
          <Route
            path="transactions"
            element={<InstitutionTransactionsListView />}
          />
        </Route>
      </Route>

      <Route path="*" element={<ErrorComponent />} />
    </Routes>
  );
}

export default AppRoutes;

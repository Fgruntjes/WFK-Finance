import { Admin, CustomRoutes, Resource } from "react-admin";
import { BrowserRouter, Route } from "react-router-dom";
import AppLayout from "./AppLayout";
import Dashboard from "./Dashboard";
import {
  InstitutionAccountShowView,
  InstitutionConnectionCreateReturnView,
  InstitutionConnectionCreateView,
  InstitutionConnectionListView,
  InstitutionRecordRepresentation,
} from "./Views";
import auth0Client from "./authClient";
import authProvider from "./authProvider";
import dataProvider from "./dataProvider";
import { i18nProvider } from "./i18nProvider";

function App() {
  if (!auth0Client) {
    return <div>Loading...</div>;
  }

  return (
    <BrowserRouter>
      <Admin
        layout={AppLayout}
        dataProvider={dataProvider}
        authProvider={authProvider}
        i18nProvider={i18nProvider}
        requireAuth
      >
        <CustomRoutes>
          <Route path="/" element={<Dashboard />} />
        </CustomRoutes>
        <Resource
          name="institutionconnections"
          list={<InstitutionConnectionListView />}
          create={<InstitutionConnectionCreateView />}
        >
          <Route
            path="create-return"
            element={<InstitutionConnectionCreateReturnView />}
          />
        </Resource>
        <Resource
          name="institutionaccounts"
          show={<InstitutionAccountShowView />}
        />
        <Resource
          name="institutions"
          recordRepresentation={<InstitutionRecordRepresentation />}
        />
      </Admin>
    </BrowserRouter>
  );
}

export default App;

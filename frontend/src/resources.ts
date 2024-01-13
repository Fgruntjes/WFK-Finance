import { ResourceProps } from "@refinedev/core";

export const instititutionconnections: ResourceProps = {
  name: "institutionconnections",
  list: "/bank-accounts",
  create: "/bank-accounts/create",
};

export const institutionaccounts: ResourceProps = {
  name: "institutionaccounts",
  show: "/bank-accounts/:id",
};

export const institutionaccounttransactions: ResourceProps = {
  name: "institutionaccounttransactions",
  list: "/bank-accounts/:accountId/transactions",
};

export const resourceList = [instititutionconnections, institutionaccounts];

import { ResourceProps } from "@refinedev/core";

export const institutions: ResourceProps = {
  name: "institutions",
};

export const institutionaccounts: ResourceProps = {
  name: "institutionaccounts",
  show: "/bank-accounts/:id",
  create: "/bank-accounts/create",
  list: "/bank-accounts",
  meta: {
    parent: "institutions",
  },
};

export const institutiontransactions: ResourceProps = {
  name: "institutiontransactions",
  list: "/bank-accounts/transactions",
  meta: {
    parent: "institutions",
  },
};

export const uncategorizedtransactions: ResourceProps = {
  name: "uncategorizedtransactions",
  list: "/bank-accounts/transactions/uncategorized",
};

export const transactioncategories: ResourceProps = {
  name: "transactioncategories",
  list: "/transaction-categories",
};

export const resourceList = [
  institutionaccounts,
  institutiontransactions,
  uncategorizedtransactions,
  transactioncategories,
];

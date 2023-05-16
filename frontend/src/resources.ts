import { ResourceProps } from "@refinedev/core";

export const resources: ResourceProps[] = [{
    name: "BankConnection",
    list: "/BankConnection",
    create: "/BankConnection/create",
    edit: "/BankConnection/edit/:id",
    show: "/BankConnection/show/:id"
}]
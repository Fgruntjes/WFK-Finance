import { ThemeIcon } from "@mantine/core";
import { ResourceProps } from "@refinedev/core";
import { IconBuildingBank } from "@tabler/icons-react";

export const resources: ResourceProps[] = [{
    name: "InstitutionConnection",
    meta: {
        label: "Institutions",
        icon: <ThemeIcon><IconBuildingBank /></ThemeIcon>,
    },
    list: "/institution",
    create: "/institution/create",
    edit: "/institution/edit/:id",
    show: "/institution/show/:id"
}]
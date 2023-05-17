export type RouteItem = {
    id: string,
    translationKey: string,
    includeInMenu: boolean,
}
type RouteMap = {[key: string]: RouteItem}

export const routes: RouteItem[] = [
    {
        id: '/',
        translationKey: 'routes.home',
        includeInMenu: false,
    },
    {
        id: '/institutionconnections',
        translationKey: 'routes.institutionconnections',
        includeInMenu: true,
    }
];

export const routesById: RouteMap = routes.reduce((acc, route) => {
    acc[route.id] = route;
    return acc;
}, {} as RouteMap);
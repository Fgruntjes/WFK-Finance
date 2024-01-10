import { Menu, MenuProps, useResourceDefinitions } from "react-admin";

function AppMenu(props: MenuProps) {
  const resources = useResourceDefinitions();
  const resourceNames = Object.keys(resources).filter(
    (name) => resources[name].hasList,
  );

  return (
    <Menu {...props}>
      <Menu.DashboardItem />
      {resourceNames.map((name) => (
        <Menu.Item key={name} name={name} data-testid={`menuitem:${name}`} />
      ))}
    </Menu>
  );
}

export default AppMenu;

import { CheckForApplicationUpdate, Layout, LayoutProps } from "react-admin";
import { useLocation } from "react-router-dom";
import AppMenu from "./AppMenu";

function AppLayout({ children, ...props }: LayoutProps) {
  const location = useLocation();

  return (
    <Layout
      {...props}
      menu={AppMenu}
      data-testid={`activeroute:${location.pathname}`}
    >
      {children}
      <CheckForApplicationUpdate />
    </Layout>
  );
}

export default AppLayout;

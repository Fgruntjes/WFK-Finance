import { CheckForApplicationUpdate, Layout, LayoutProps } from "react-admin";

function AppLayout({ children, ...props }: LayoutProps) {
  return (
    <Layout {...props}>
      {children}
      <CheckForApplicationUpdate />
    </Layout>
  );
}

export default AppLayout;

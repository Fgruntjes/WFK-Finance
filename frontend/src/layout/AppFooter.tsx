import { useParsed } from "@refinedev/core";
import { Flex } from "antd";
import { Footer } from "antd/es/layout/layout";

function AppFooter() {
  const { pathname } = useParsed();
  const year = new Date().getFullYear();
  return (
    <Footer style={{ padding: 2 }} data-testid={`activeroute:${pathname}`}>
      <Flex
        justify={"center"}
        style={{
          fontSize: 14,
          color: "#777",
        }}
      >
        © {year} WFK-Finance - All rights reserved.
      </Flex>
    </Footer>
  );
}
export default AppFooter;

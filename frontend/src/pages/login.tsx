import { Box, Button, Space, Text } from "@mantine/core";
import { useTranslate } from "@refinedev/core";
import { ThemedTitleV2 } from "@refinedev/mantine";

import { useAuth0 } from "@auth0/auth0-react";

export const Login: React.FC = () => {
  const { loginWithRedirect } = useAuth0();

  const t = useTranslate();

  return (
    <Box
      sx={{
        height: "100vh",
        display: "flex",
        flexDirection: "column",
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      <ThemedTitleV2
        collapsed={false}
        text="WFK Finance"
        wrapperStyles={{
          fontSize: "22px",
        }}
      />
      <Space h="xl" />

      <Button
        style={{ width: "240px" }}
        type="button"
        variant="filled"
        onClick={() => loginWithRedirect()}
      >
        {t("pages.login.signin", "Sign in")}
      </Button>
      <Space h="xl" />
      <Text fz="sm" color="gray">
        Powered by
        <img
          style={{ padding: "0 5px" }}
          alt="Auth0"
          src="https://refine.ams3.cdn.digitaloceanspaces.com/superplate-auth-icons%2Fauth0-2.svg"
        />
        Auth0
      </Text>
    </Box>
  );
};

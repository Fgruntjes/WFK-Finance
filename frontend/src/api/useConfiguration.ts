import { useAuth0 } from "@auth0/auth0-react";
import { Configuration } from "./generated";

function useConfiguration() {
  const { getAccessTokenSilently } = useAuth0();
  return new Configuration({
    basePath: import.meta.env.APP_API_BASE_PATH,
    accessToken: () => getAccessTokenSilently(),
  });
}

export default useConfiguration;
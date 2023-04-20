import { useAuth0 } from "@auth0/auth0-vue";
import { Configuration } from "./generated";

const configuration = new Configuration({
  basePath: import.meta.env.APP_API_BASE_PATH,
  accessToken: () => {
    const auth0 = useAuth0();
    return auth0.getAccessTokenSilently();
  },
});

export default configuration;
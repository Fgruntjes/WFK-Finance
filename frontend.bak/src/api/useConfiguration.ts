import { Configuration } from "./generated";

function useConfiguration() {
  return new Configuration({
    basePath: import.meta.env.APP_API_BASE_PATH,
  });
}

export default useConfiguration;
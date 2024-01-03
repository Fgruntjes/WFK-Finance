import react from "@vitejs/plugin-react-swc";
import path from "path";
import { defineConfig, loadEnv } from "vite";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  // Load app-level env vars to node-level env vars.
  process.env = { ...process.env, ...loadEnv(mode, process.cwd(), "") };
  const isProduction = process.env.NODE_ENV === "production";

  if (!isProduction) {
    console.log("Starting env", process.env);
  }

  function envVal(name: string, defaultValue: string | undefined = undefined) {
    if (!process.env[name]) {
      return defaultValue || `__MISSING_ENV_VAR__${name}__`;
    }

    return JSON.stringify(process.env[name]);
  }

  const appUrl = envVal("APP_FRONTEND_URL", "http://localhost:3000");
  return {
    server: {
      port: 3000,
    },
    plugins: [react()],
    define: {
      "process.env": {},
      "import.meta.env.APP_API_URI": envVal("APP_API_URI"),
      "import.meta.env.AUTH0_DOMAIN": envVal("AUTH0_DOMAIN"),
      "import.meta.env.AUTH0_CLIENT_ID": envVal("AUTH0_CLIENT_ID"),
      "import.meta.env.AUTH0_AUDIENCE": envVal("AUTH0_AUDIENCE"),
      "import.meta.env.LOGIN_REDIRECT_URL": JSON.stringify(
        `${appUrl}/auth-callback`,
      ),
      "import.meta.env.LOGOUT_REDIRECT_URL": JSON.stringify(`${appUrl}`),
      "import.meta.env.SENTRY_DSN": envVal("SENTRY_DSN"),
    },
    build: {
      sourcemap: process.env.NODE_ENV !== "production",
    },
    resolve: {
      alias: {
        "@Api": path.resolve(__dirname, "./src/Api.ts"),
        "@Components": path.resolve(__dirname, "./src/Components"),
        "@Views": path.resolve(__dirname, "./src/Views"),
      },
    },
  };
});

import react from "@vitejs/plugin-react";
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

  function envVal(name: string) {
    if (!process.env[name]) {
      // We replace these vars in the deploy step with the actual values.
      return `__MISSING_ENV_VAR__${name}__`;
    }

    return JSON.stringify(process.env[name]);
  }

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
      "import.meta.env.SENTRY_DSN": envVal("SENTRY_DSN"),
    },
    build: {
      sourcemap: process.env.NODE_ENV !== "production",
    },
    resolve: {
      alias: {
        "@api": `${path.resolve(__dirname, "./src")}/Api.ts`,
        "@components": path.resolve(__dirname, "./src/components"),
        "@pages": path.resolve(__dirname, "./src/pages"),
        "@hooks": path.resolve(__dirname, "./src/hooks"),
      },
    },
  };
});

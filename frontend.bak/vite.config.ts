import generouted from '@generouted/react-router/plugin';
import react from '@vitejs/plugin-react-swc';
import autoprefixer from "autoprefixer";
import { fileURLToPath } from 'url';
import { defineConfig, loadEnv } from 'vite';

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  // Load app-level env vars to node-level env vars.
  process.env = {...process.env, ...loadEnv(mode, process.cwd(), '')};
    
  return {
    server: {
      port: 3000,
    },
    plugins: [
      react(),
      generouted(),
    ],
    css: {
      postcss: {
        plugins: [autoprefixer],
      },
    },
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
      },
    },
    define: {
      'process.env': {},
      'import.meta.env.APP_API_BASE_PATH': JSON.stringify(process.env.APP_API_BASE_PATH),
      'import.meta.env.AUTH0_DOMAIN': JSON.stringify(process.env.AUTH0_DOMAIN),
      'import.meta.env.AUTH0_AUDIENCE': JSON.stringify(process.env.AUTH0_AUDIENCE),
      'import.meta.env.AUTH0_SCOPE': JSON.stringify(process.env.AUTH0_SCOPE),
      'import.meta.env.AUTH0_CLIENT_ID': JSON.stringify(process.env.AUTH0_CLIENT_ID)
    }
  }
});

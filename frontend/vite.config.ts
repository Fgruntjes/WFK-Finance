import vueI18nPlugin from '@intlify/unplugin-vue-i18n/vite'
import vue from '@vitejs/plugin-vue'
import vueJsx from '@vitejs/plugin-vue-jsx'
import autoImport from 'unplugin-auto-import/vite'
import components from 'unplugin-vue-components/vite'
import defineOptions from 'unplugin-vue-define-options/vite'
import { fileURLToPath } from 'url'
import { defineConfig, loadEnv } from 'vite'
import pages from 'vite-plugin-pages'
import layouts from 'vite-plugin-vue-layouts'
import vuetify from 'vite-plugin-vuetify'

// https://vitejs.dev/config/
export default defineConfig(({ command, mode, ssrBuild }) => {
  // Load app-level env vars to node-level env vars.
  process.env = {...process.env, ...loadEnv(mode, process.cwd(), '')};
    
  return {
    server: {
      port: 3000,
    },
    plugins: [
      vue(),
      vueJsx(),
  
      // https://github.com/vuetifyjs/vuetify-loader/tree/next/packages/vite-plugin
      vuetify({
        styles: {
          configFile: 'src/styles/variables/_vuetify.scss',
        },
      }),
      pages(),
      layouts(),
      components({
        dirs: ['src/@core/components'],
        dts: true,
      }),
      autoImport({
        imports: [
          'vue',
          'vue-router',
          '@vueuse/core',
          'vue-i18n',
          'pinia',
          {
            'vue-request':
            [
              'useRequest',
              'usePagination',
              'useLoadMore',
            ]
          },
        ],
        vueTemplate: true,
        eslintrc: { enabled: true }
      }),
      defineOptions(),
      vueI18nPlugin({}),
    ],
    define: {
      __VUE_I18N_FULL_INSTALL__: true,
      __VUE_I18N_LEGACY_API__: false,
      __INTLIFY_PROD_DEVTOOLS__: false,
      'import.meta.env.AUTH0_DOMAIN': JSON.stringify(process.env.AUTH0_DOMAIN),
      'import.meta.env.AUTH0_AUDIENCE': JSON.stringify(process.env.AUTH0_AUDIENCE),
      'import.meta.env.AUTH0_SCOPE': JSON.stringify(process.env.AUTH0_SCOPE),
      'import.meta.env.AUTH0_CLIENT_ID': JSON.stringify(process.env.AUTH0_CLIENT_ID),
      'import.meta.env.APP_API_BASE_PATH': JSON.stringify(process.env.APP_API_BASE_PATH),
     },
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
        '@core': fileURLToPath(new URL('./src/@core', import.meta.url)),
        '@layouts': fileURLToPath(new URL('./src/@layouts', import.meta.url)),
        '@configured-variables': fileURLToPath(new URL('./src/styles/variables/_template.scss', import.meta.url)),
        'apexcharts': fileURLToPath(new URL('node_modules/apexcharts-clevision', import.meta.url)),
      },
    },
    build: {
      chunkSizeWarningLimit: 5000,
    },
    optimizeDeps: {
      exclude: ['vuetify'],
      entries: [
        './src/**/*.vue',
      ],
    },
  };
})

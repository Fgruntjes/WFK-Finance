import { sveltekit } from '@sveltejs/kit/vite';
import { optimizeCss } from 'carbon-preprocess-svelte';
import { defineConfig, loadEnv } from 'vite';
import houdini from 'houdini/vite'

export default defineConfig(({ mode }) => {
	// Load app-level env vars to node-level env vars.
	process.env = { ...process.env, ...loadEnv(mode, process.cwd(), '') };

	return {
		server: {
			port: 3000
		},
		plugins: [
			houdini(),
			sveltekit(),
			process.env.NODE_ENV === 'production' && optimizeCss()
		],
		define: {
			'process.env': {},
			'import.meta.env.APP_API_URI': JSON.stringify(process.env.APP_API_URI),
			'import.meta.env.AUTH0_DOMAIN': JSON.stringify(process.env.AUTH0_DOMAIN),
			'import.meta.env.AUTH0_AUDIENCE': JSON.stringify(process.env.AUTH0_AUDIENCE),
			'import.meta.env.AUTH0_SCOPE': JSON.stringify(process.env.AUTH0_SCOPE),
			'import.meta.env.AUTH0_CLIENT_ID': JSON.stringify(process.env.AUTH0_CLIENT_ID)
		},
		optimizeDeps: {
			exclude: ['@urql/svelte'],
		},
		build: {
			sourcemap: process.env.NODE_ENV !== 'production'
		}
	};
});

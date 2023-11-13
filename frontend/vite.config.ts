import { sveltekit } from '@sveltejs/kit/vite';
import { optimizeCss } from 'carbon-preprocess-svelte';
import { defineConfig, loadEnv } from 'vite';
import houdini from 'houdini/vite';

export default defineConfig(({ mode }) => {
	// Load app-level env vars to node-level env vars.
	process.env = { ...process.env, ...loadEnv(mode, process.cwd(), '') };
	const isProduction = process.env.NODE_ENV === 'production';

	if (!isProduction) {
		console.log('Starting env', process.env);
	}

	function envVal(name: string) {
		if (isProduction) {
			return `__${name}__`;
		}

		if (!process.env[name]) {
			return `__MISSING__${name}__`;
		}

		return JSON.stringify(process.env[name]);
	}

	return {
		server: {
			port: 3000
		},
		plugins: [houdini(), sveltekit(), isProduction && optimizeCss()],
		define: {
			'process.env': {},
			'import.meta.env.APP_API_URI': envVal('APP_API_URI'),
			'import.meta.env.AUTH0_DOMAIN': envVal('AUTH0_DOMAIN'),
			'import.meta.env.AUTH0_CLIENT_ID': envVal('AUTH0_CLIENT_ID'),
			'import.meta.env.AUTH0_AUDIENCE': envVal('AUTH0_AUDIENCE')
		},
		optimizeDeps: {
			exclude: ['@urql/svelte']
		},
		build: {
			sourcemap: process.env.NODE_ENV !== 'production'
		}
	};
});

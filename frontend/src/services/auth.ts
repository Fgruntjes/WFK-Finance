import { goto } from '$app/navigation';
import {
	Auth0Client,
	User,
	createAuth0Client,
	type GetTokenSilentlyOptions,
	type LogoutOptions,
	type RedirectLoginOptions
} from '@auth0/auth0-spa-js';
import { get, writable } from 'svelte/store';

type AppState = {
	targetUrl?: string;
};

const initializeUseAuth0 = () => {
	const auth0Client = writable<Auth0Client | null>(null);
	const isAuthenticated = writable(false);
	const isLoading = writable(true);
	const user = writable<User | null>(null);
	const error = writable<Error | null>(null);

	const initializeAuth = async () => {
		auth0Client.set(
			await createAuth0Client({
				domain: import.meta.env.AUTH0_DOMAIN,
				clientId: import.meta.env.AUTH0_CLIENT_ID,
				useRefreshTokens: true,
				authorizationParams: {
					redirect_uri: window.location.origin,
					scope: import.meta.env.AUTH0_SCOPE,
					audience: import.meta.env.AUTH0_AUDIENCE
				}
			})
		);
		try {
			const search = window.location.search;

			if ((search.includes('code=') || search.includes('error=')) && search.includes('state=')) {
				const loginResult = await get(auth0Client)?.handleRedirectCallback<AppState>();

				goto(loginResult?.appState?.targetUrl ?? window.location.pathname, { replaceState: true });
			}
		} catch (err) {
			error.set(err as Error);
		} finally {
			isAuthenticated.set((await get(auth0Client)?.isAuthenticated()) || false);
			user.set((await get(auth0Client)?.getUser()) || null);
			isLoading.set(false);
		}
	};

	const login = async (options?: RedirectLoginOptions) => {
		await get(auth0Client)?.loginWithRedirect({
			appState: { targetUrl: window.location.pathname },
			...options
		});
	};

	const logout = async (options?: LogoutOptions) => {
		get(auth0Client)?.logout(options);
	};

	const getAccessToken = async (options?: GetTokenSilentlyOptions) => {
		return await get(auth0Client)?.getTokenSilently(options);
	};

	return {
		isAuthenticated,
		isLoading,
		user,
		error,

		initializeAuth,
		login,
		logout,
		getAccessToken
	};
};

export const auth = initializeUseAuth0();

import { goto } from '$app/navigation';
import {
	Auth0Client,
	User,
	createAuth0Client,
	type LogoutOptions,
	type RedirectLoginOptions
} from '@auth0/auth0-spa-js';
import { get, writable } from 'svelte/store';

type AppState = {
	targetUrl?: string;
};

const initializeUseAuth0 = () => {
	const auth0ClientStore = writable<Auth0Client | null>(null);
	const isAuthenticatedStore = writable(false);
	const isLoadingStore = writable(true);
	const userStore = writable<User | null>(null);
	const errorStore = writable<Error | null>(null);
	const accessTokenStore = writable<string | null>(null);

	return {
		isAuthenticated: isAuthenticatedStore,
		isLoading: isLoadingStore,
		accessToken: accessTokenStore,
		user: userStore,
		error: errorStore,

		initializeAuth: async () => {
			const auth0Client = await createAuth0Client({
				domain: import.meta.env.AUTH0_DOMAIN,
				clientId: import.meta.env.AUTH0_CLIENT_ID,
				useRefreshTokens: true,
				authorizationParams: {
					redirect_uri: window.location.origin,
					audience: import.meta.env.AUTH0_AUDIENCE,
				}
			});
			auth0ClientStore.set(auth0Client);

			try {
				const search = window.location.search;

				if ((search.includes('code=') || search.includes('error=')) && search.includes('state=')) {
					const loginResult = await auth0Client.handleRedirectCallback<AppState>();

					goto(loginResult?.appState?.targetUrl ?? window.location.pathname, { replaceState: true });
				}
			} catch (err) {
				errorStore.set(err as Error);
			} finally {
				const isAuthenticated = await auth0Client.isAuthenticated();
				isAuthenticatedStore.set(isAuthenticated || false);
				if (isAuthenticated) {
					accessTokenStore.set(await auth0Client.getTokenSilently() || null);
				} else {
					accessTokenStore.set(null);
				}

				userStore.set((await auth0Client.getUser()) || null);
				isLoadingStore.set(false);
			}
		},
		login: async (options?: RedirectLoginOptions) => {
			await get(auth0ClientStore)?.loginWithRedirect({
				appState: { targetUrl: window.location.pathname + window.location.search },
				...options
			});
		},
		logout: async (options?: LogoutOptions) => {
			get(auth0ClientStore)?.logout(options);
		},
	};
};

export const auth = initializeUseAuth0();

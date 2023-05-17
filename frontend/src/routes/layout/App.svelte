<script lang="ts">
	import { QueryClient, QueryClientProvider } from '@tanstack/svelte-query';
	import { Content } from 'carbon-components-svelte';
	import AuthGuard from './AuthGuard.svelte';
	import Header from './Header.svelte';
	import I18nLoader from './I18nLoader.svelte';

	const queryClient = new QueryClient();
</script>

<I18nLoader>
	<AuthGuard>
		<QueryClientProvider client={queryClient}>
			<Header />
			<Content>
				<slot />
			</Content>
		</QueryClientProvider>
	</AuthGuard>
</I18nLoader>

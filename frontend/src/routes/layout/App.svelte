<script lang="ts">
	import { Content } from 'carbon-components-svelte';
	import AuthGuard from './AuthGuard.svelte';
	import Header from './Header.svelte';
	import I18nLoader from './I18nLoader.svelte';
	import client from '@/api/client';
	import { setContextClient } from '@urql/svelte';

	setContextClient(client);
</script>

<I18nLoader>
	<AuthGuard>
		<Header />
		<Content>
			<slot />
		</Content>
	</AuthGuard>
</I18nLoader>

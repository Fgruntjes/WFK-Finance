<script lang="ts">
	import Loader from '@/components/Loader.svelte';
	import { auth } from '@/services/auth';
	import { onMount } from 'svelte';

	let { isLoading, isAuthenticated, login, initializeAuth, error } = auth;

	onMount(async () => {
		await initializeAuth();

		if (!$isAuthenticated) {
			login();
		}
	});
</script>

{#if $error}
	<p>
		Auth error: {$error.message}
	</p>
{:else if $isLoading || !$isAuthenticated}
	<Loader />
{:else}
	<slot />
{/if}

<script lang="ts">
	import { page } from '$app/stores';
	import Loader from '@/components/Loader.svelte';
	import { auth } from '@/services/auth';
	import { onMount } from 'svelte';

	let { isLoading, isAuthenticated, login, initializeAuth, error } = auth;

	onMount(async () => {
		await initializeAuth();

		if (!$isAuthenticated && !$page.error && !$error) {
			login();
		}
	});

	$: {
		if (!isLoading && !$isAuthenticated && !$page.error && !$error) {
			login();
		}
	}
</script>

{#if $error}
	<p>
		Auth error: {$error.message}
	</p>
{:else if $page.error}
	<slot />
{:else if $isLoading || !$isAuthenticated}
	<Loader />
{:else}
	<slot />
{/if}

<script lang="ts">
	import Loader from '@/components/Loader.svelte';
	import { isLoading } from 'svelte-i18next';
</script>

{#if $isLoading}
	<Loader />
{:else}
	<slot />
{/if}

<script lang="ts">
	import type { CreateQueryResult } from '@tanstack/svelte-query';
	import LocalError from './LocalError.svelte';

	export let query: CreateQueryResult<any, Error>;
</script>

{#if $query.error}
	<LocalError error={$query.error} />
{/if}
{#if $query.isSuccess}
	<slot />
{/if}

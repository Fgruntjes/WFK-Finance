<script lang="ts">
	import type { InstitutionConnection } from '@/api/generated';
	import queries from '@/api/queries';
	import Button from '@/components/Button.svelte';
	import { i18n } from '@/services/i18n';
	import { createQuery } from '@tanstack/svelte-query';
	import type { DataTableRow } from 'carbon-components-svelte/types/DataTable/DataTable.svelte';
	import { Renew as RenewIcon } from 'carbon-icons-svelte';
	import { createEventDispatcher } from 'svelte';

	export let institutionConnection: InstitutionConnection | DataTableRow;

	const institutionRefresh = createQuery({
		...queries.institutionConnection.refresh({ id: institutionConnection.id }),
		enabled: false,
		onSettled: (data) => data && dispatchDelete(data)
	});

	const dispatch = createEventDispatcher();
	const dispatchDelete = (connection: InstitutionConnection) => dispatch('refresh', connection);
</script>

<Button
	icon={RenewIcon}
	title={$i18n.t('institutionconnections:list.actions.refresh')}
	kind="ghost"
	iconOnly
	isLoading={$institutionRefresh.isFetching}
	on:click={() => $institutionRefresh.refetch()}
/>

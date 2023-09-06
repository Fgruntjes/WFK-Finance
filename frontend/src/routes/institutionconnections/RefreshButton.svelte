<script lang="ts">
	import Button from '@/components/Button.svelte';
	import { i18n } from '@/services/i18n';
	import type { DataTableRow } from 'carbon-components-svelte/types/DataTable/DataTable.svelte';
	import { Renew as RenewIcon } from 'carbon-icons-svelte';
	import { createEventDispatcher } from 'svelte';

	export let institutionConnection: InstitutionConnection | DataTableRow;

	const dispatch = createEventDispatcher();
	const dispatchDelete = (connection: InstitutionConnection) => dispatch('refresh', connection);
</script>

<Button
	icon={RenewIcon}
	title={$i18n.t('institutionconnections:list.actions.refresh')}
	kind="ghost"
	iconOnly
	isLoading={true}
	on:click={() => {}}
/>

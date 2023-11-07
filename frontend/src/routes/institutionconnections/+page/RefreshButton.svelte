<script lang="ts">
	import Button from '@/components/Button.svelte';
	import { i18n } from '@/services/i18n';
	import { Renew as RenewIcon } from 'carbon-icons-svelte';

	export let isLoading = false;
	// export let institutionConnection: InstitutionConnection | DataTableRow;

	// const dispatch = createEventDispatcher();
	// const dispatchDelete = (connection: InstitutionConnection) => dispatch('refresh', connection);
</script>

<Button
	icon={RenewIcon}
	title={$i18n.t('institutionconnections:list.actions.refresh')}
	kind="ghost"
	iconOnly
	{isLoading}
	on:click
/>

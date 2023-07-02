<script lang="ts">
	import { i18n } from '@/services/i18n';
	import { Modal } from 'carbon-components-svelte';
	import { TrashCan as DeleteIcon } from 'carbon-icons-svelte';
	import { createEventDispatcher } from 'svelte';
	import Button from '../Button.svelte';

	type TQueryFunctionData = $$Generic;
	type TQueryKey = $$Generic<QueryKey>;

	export let title: string;
	export let confirmation: string = $i18n.t('components.deletebutton.confirmation');
	const showConfirmation = !!confirmation;

	const dispatch = createEventDispatcher();
	const dispatchDelete = () => dispatch('delete');

	let confirmationOpen = false;
	const handleClick = () => {
		if (showConfirmation) {
			confirmationOpen = true;
		} else {
			dispatchDelete();
		}
	};
</script>

{#if showConfirmation}
	<Modal
		danger
		bind:open={confirmationOpen}
		modalHeading={title}
		primaryButtonText={$i18n.t('components.deletebutton.delete')}
		secondaryButtonText={$i18n.t('components.deletebutton.cancel')}
		on:click:button--secondary={() => (confirmationOpen = false)}
		on:click:button--primary={() => {
			dispatchDelete();
			confirmationOpen = false;
		}}
	>
		<p>{confirmation}</p>
	</Modal>
{/if}

<Button icon={DeleteIcon} kind="danger" {title} on:click={() => handleClick()} {...$$restProps} />

<script lang="ts">
	import { i18n } from '@/services/i18n';
	import { TrashCan as DeleteIcon } from 'carbon-icons-svelte';
	import Button from '../Button.svelte';

	const { ...buttonProps } = $$props;

	// TODO implement confirmation dialog
</script>

<Button
	icon={DeleteIcon}
	kind="danger"
	title={buttonProps.title || $i18n.t('table.actions.delete')}
	{...buttonProps}
/>

<script lang="ts">
	import { i18n } from '@/services/i18n';
	import { Add as AddIcon } from 'carbon-icons-svelte';
	import Button from '../Button.svelte';

	const { ...buttonProps } = $$props;
</script>

<Button icon={AddIcon} title={buttonProps.title || $i18n.t('table.actions.add')} {...buttonProps} />

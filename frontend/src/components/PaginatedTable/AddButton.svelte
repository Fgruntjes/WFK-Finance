<script lang="ts">
	import { i18n } from '@/services/i18n';
	import { Add as AddIcon } from 'carbon-icons-svelte';
	import Button from '../Button.svelte';

	export let route: string;
	export let title: string | undefined = undefined;
</script>

<Button
	href={route}
	icon={AddIcon}
	title={title || $i18n.t('components.addbutton.label')}
	{...$$restProps}
/>

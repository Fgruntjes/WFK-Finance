<script lang="ts">
	import { Button, InlineLoading } from 'carbon-components-svelte';

	export let isLoading: boolean = false;
	export let disabled: boolean = false;
	export let iconOnly: boolean = false;
	export let icon: typeof import('svelte').SvelteComponent | undefined = undefined;
	export let title: string;

	if (iconOnly && isLoading) {
		icon = InlineLoading;
	}

	$: buttonProps = {
		icon,
		disabled: disabled || isLoading,
		iconDescription: title,
		...$$restProps
	};
</script>

{#if iconOnly}
	<Button on:click {...buttonProps} />
{:else}
	<Button on:click {...buttonProps}>
		{#if isLoading}
			<InlineLoading />
		{/if}
		{title}
		<slot />
	</Button>
{/if}

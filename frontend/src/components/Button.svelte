<script lang="ts" context="module">
	import type { ButtonProps as CarbonButtonProps } from 'carbon-components-svelte/types/Button/Button.svelte';
	export type ButtonProps = CarbonButtonProps & {
		isLoading?: boolean;
		title?: string;
	};
</script>

<script lang="ts">
	import { Button, InlineLoading } from 'carbon-components-svelte';

	const { isLoading, title, ...buttonProps }: ButtonProps = $$props;
</script>

<Button {...buttonProps} disabled={buttonProps?.disabled || isLoading} iconDescription={title}>
	{#if isLoading}
		<InlineLoading />
	{/if}
	{title}
	<slot />
</Button>

<script lang="ts">
	import { ToastNotification } from 'carbon-components-svelte';

	export let error: unknown;
	let normalizedError: Error | undefined;

	if (error instanceof Error) {
		normalizedError = error;
	} else if (typeof error === 'string') {
		normalizedError = new Error(error);
	} else if (typeof error == 'object' && error !== null) {
		if ('message' in error) {
			normalizedError = new Error((error.message as string) ?? undefined);
		} else {
			normalizedError = new Error('Unknown error');
		}
	} else {
		normalizedError = new Error('Unknown error');
	}

	const caption = new Date().toLocaleString();
	const title = normalizedError.name ?? 'Error';
	const subtitle = normalizedError.message ?? 'Unknown error';
</script>

<ToastNotification hideCloseButton fullWidth {title} {subtitle} {caption} />

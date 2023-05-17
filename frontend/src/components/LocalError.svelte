<script lang="ts">
	import { ToastNotification } from 'carbon-components-svelte';

	export let error: Error;

	const caption = new Date().toLocaleString();
	const title = error.name ?? 'Error';
	const subtitle = error.message ?? 'Unknown error';
</script>

<ToastNotification hideCloseButton fullWidth {title} {subtitle} {caption} />

<script lang="ts">
	import { ToastNotification } from 'carbon-components-svelte';

	export let error: unknown;

	const normalizeErrors = (error: unknown): Error[] => {
		if (Array.isArray(error)) {
			return error.map(normalizeErrors).flat();
		}

		if (error instanceof Error) {
			return [error];
		} else if (typeof error === 'string') {
			return [new Error(error)];
		} else if (typeof error == 'object' && error !== null) {
			if ('message' in error) {
				return [new Error((error.message as string) ?? undefined)];
			}
			return [new Error('Unknown error')];
		}

		return [new Error('Unknown error')];
	};

	const normalizedErrors = normalizeErrors(error);
	const caption = new Date().toLocaleString();
	const title = normalizedErrors.length === 1 ? normalizedErrors[0].name ?? 'Error' : 'Errors';
	const subtitle =
		normalizedErrors.length === 1
			? normalizedErrors[0].message ?? 'Unknown error'
			: normalizedErrors.map((error) => error.message).join('\n');
</script>

<ToastNotification hideCloseButton fullWidth {title} {subtitle} {caption} />

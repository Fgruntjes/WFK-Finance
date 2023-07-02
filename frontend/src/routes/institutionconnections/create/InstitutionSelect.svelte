<script lang="ts">
	import type { Institution } from '@/api/generated';
	import { institutionQuery } from '@/api/queries/institutionQuery';
	import { i18n } from '@/services/i18n';
	import { createQuery } from '@tanstack/svelte-query';
	import { Select, SelectItem, SelectSkeleton } from 'carbon-components-svelte';

	export let country: string;
	export let selected: Institution | undefined = undefined;
	let selectedId: string | undefined = undefined;

	$: optionsQuery = createQuery({
		...institutionQuery.search({ country })
	});

	let options: { [key: string]: Institution } = {};
	$: {
		if (selectedId) {
			selected = options[selectedId];
		} else {
			selected = undefined;
		}

		options = {};
		$optionsQuery.data
			?.sort((a, b) => {
				const nameA = a.name;
				const nameB = b.name;
				if (!nameA) {
					if (nameB) return -1;
					return 0;
				}

				if (!nameB) {
					if (nameA) return 1;
					return 0;
				}

				return nameA.localeCompare(nameB);
			})
			.forEach((i) => (options[i.id] = i));
	}
</script>

{#if $optionsQuery.isLoading}
	<SelectSkeleton />
{:else}
	<Select
		labelText={$i18n.t('institutionconnections:create.bankselect')}
		bind:selected={selectedId}
		{...$$restProps}
	>
		<SelectItem text="" value="" />
		{#each Object.values(options) as institution}
			<SelectItem text={institution.name} value={institution.id} />
		{/each}
	</Select>
{/if}

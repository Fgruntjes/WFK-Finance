<script lang="ts">
	import { graphql } from '$houdini';
	import type { InstitutionsVariables } from './$houdini';
	import { i18n } from '@/services/i18n';
	import { Select, SelectItem, SelectSkeleton } from 'carbon-components-svelte';

	export let countryIso2: string;
	export let selectedId: Guid | undefined = undefined;
	export const _InstitutionsVariables: InstitutionsVariables = ({ props: { countryIso2 } }) => {
		return { countryIso2 };
	};

	const optionsStore = graphql(`
		query Institutions($countryIso2: String!) @load {
			institution {
				list(countryIso2: $countryIso2) {
					id
					name
				}
			}
		}
	`);

	let options: { [key: string]: { id: Guid; name: string } } = {};
	$: {
		options = {};
		$optionsStore.data?.institution?.list
			?.sort((a, b) => {
				const nameA = a.name;
				const nameB = b.name;
				if (!nameA) {
					if (!!nameB) return -1;
					return 0;
				}

				if (!nameB) {
					if (!!nameA) return 1;
					return 0;
				}

				return nameA.localeCompare(nameB);
			})
			.forEach((i) => (options[i.id] = i));
	}
</script>

{#if $optionsStore.fetching}
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

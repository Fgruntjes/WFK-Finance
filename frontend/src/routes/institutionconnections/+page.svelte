<script lang="ts">
	import type { Institution, ListResponseOfInstitutionConnection } from '@/api/generated';
	import { queries } from '@/api/queries';
	import { institutionQuery } from '@/api/queries/institutionQuery';
	import LocalError from '@/components/LocalError.svelte';
	import PageTitle from '@/components/PageTitle.svelte';
	import PaginatedTable from '@/components/PaginatedTable/PaginatedTable.svelte';
	import { i18n } from '@/services/i18n';
	import { createQuery, type CreateQueryResult } from '@tanstack/svelte-query';
	import { SkeletonText } from 'carbon-components-svelte';

	const tableHeaders = [
		{ key: 'institutionId', value: $i18n.t('institutionconnections:header.institution') },
		{ key: 'accounts', value: $i18n.t('institutionconnections:header.accounts') }
	];

	let listQuery: CreateQueryResult<ListResponseOfInstitutionConnection, Error>;
	let institutionData: CreateQueryResult<Array<Institution>, Error>;
	let institutionMap: Record<string, Institution> = {};
	$: {
		const institutionIds =
			$listQuery?.data?.items
				.map((item) => item.institutionId)
				.filter((value, index, array) => array.indexOf(value) === index) ?? [];

		institutionData = createQuery({
			...institutionQuery.getMany({ id: institutionIds }),
			enabled: institutionIds.length > 0
		});

		$institutionData.data?.reduce((map, institution) => {
			map[institution.id] = institution;
			return map;
		}, institutionMap);
	}
</script>

<PageTitle title="Bank accounts" />

{#if $institutionData?.error}
	<LocalError error={$institutionData?.error} />
{/if}

<PaginatedTable
	headers={tableHeaders}
	title={$i18n.t('institutionconnections:title')}
	description={$i18n.t('institutionconnections:description')}
	selectable
	batchSelection
	listAction={queries.InstitutionConnectionQuery.list}
	bind:listQuery
>
	<svelte:fragment slot="cell" let:row let:cell>
		{#if cell.key === 'institutionId'}
			{#if $institutionData.isLoading}
				<SkeletonText />
			{:else}
				{@const institution = institutionMap[cell.value]}
				{#if institution?.logo}
					<img alt={institution.name} src={institution.logo} class="institution-logo" />
				{/if}

				{institution?.name}
			{/if}
		{:else if cell.key === 'accounts'}
			<ul>
				{#each cell.value as account}
					<li>
						{account.iban}
						{#if account.ownerName}({account.ownerName}){/if}
					</li>
				{/each}
			</ul>
		{:else}
			{cell.value}
		{/if}
	</svelte:fragment>
</PaginatedTable>

<style lang="scss">
	.institution-logo {
		// make the image round and no bigger then the text in the cell
		border-radius: 10%;
		max-height: 2em;
		max-width: 2em;
	}
</style>

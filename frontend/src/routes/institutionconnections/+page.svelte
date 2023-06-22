<script lang="ts">
	import type { Institution } from '@/api/generated';
	import { institutionConnectionQuery } from '@/api/queries/institutionConnectionQuery';
	import { institutionQuery } from '@/api/queries/institutionQuery';
	import LocalError from '@/components/LocalError.svelte';
	import PageTitle from '@/components/PageTitle.svelte';
	import { i18n } from '@/services/i18n';
	import { createQuery, type CreateQueryResult } from '@tanstack/svelte-query';
	import {
		DataTable,
		DataTableSkeleton,
		Pagination,
		PaginationSkeleton,
		SkeletonText
	} from 'carbon-components-svelte';

	let currentPage = 1;
	let pageSize = 25;
	const tableHeaders = [
		{ key: 'id', value: $i18n.t('table.header.actions') },
		{ key: 'institutionId', value: $i18n.t('institutionconnections:table.header.institution') },
		{ key: 'accounts', value: $i18n.t('institutionconnections:table.header.accounts') }
	];

	const institutionConnectionData = createQuery({
		...institutionConnectionQuery.list({
			skip: (currentPage - 1) * pageSize,
			limit: pageSize
		})
	});

	let institutionData: CreateQueryResult<Array<Institution>, Error>;
	$: {
		const institutionIds =
			$institutionConnectionData.data?.items
				.map((item) => item.institutionId)
				.filter((value, index, array) => array.indexOf(value) === index) ?? [];

		institutionData = createQuery({
			...institutionQuery.getMany({ id: institutionIds }),
			enabled: institutionIds.length > 0
		});
	}

	let institutionMap: Record<string, Institution> = {};
	$: {
		$institutionData.data?.reduce((map, institution) => {
			map[institution.id] = institution;
			return map;
		}, institutionMap);
	}
</script>

<PageTitle title="Bank accounts" />

{#if $institutionConnectionData.error}
	<LocalError error={$institutionConnectionData.error} />
{/if}

{#if $institutionData?.error}
	<LocalError error={$institutionData?.error} />
{/if}

{#if $institutionConnectionData.isLoading}
	<DataTableSkeleton headers={tableHeaders} rows={pageSize} />
{:else}
	<DataTable headers={tableHeaders} rows={$institutionConnectionData.data?.items}>
		<svelte:fragment slot="cell" let:row let:cell>
			{#if cell.key === 'id'}
				<a href="/institutionconnections/{cell.value}/edit">Edit</a>
			{:else if cell.key === 'institutionId'}
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
	</DataTable>
{/if}
{#if $institutionConnectionData.isLoading}
	<PaginationSkeleton />
{:else}
	<Pagination
		pageSizes={[pageSize]}
		totalItems={$institutionConnectionData.data?.itemsTotal}
		bind:page={currentPage}
		{pageSize}
	/>
{/if}

<style lang="scss">
	.institution-logo {
		// make the image round and no bigger then the text in the cell
		border-radius: 10%;
		max-height: 2em;
		max-width: 2em;
	}
</style>

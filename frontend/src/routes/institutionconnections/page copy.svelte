<script lang="ts">
	import type { DeleteResponse, Institution } from '@/api/generated';
	import { queries } from '@/api/queries';
	import { institutionQuery } from '@/api/queries/institutionQuery';
	import LocalError from '@/components/LocalError.svelte';
	import PageTitle from '@/components/PageTitle.svelte';
	import AddButton from '@/components/PaginatedTable/AddButton.svelte';
	import DeleteButton from '@/components/PaginatedTable/DeleteButton.svelte';
	import { i18n } from '@/services/i18n';
	import {
		createMutation,
		createQuery,
		type CreateMutationResult,
		type CreateQueryResult
	} from '@tanstack/svelte-query';
	import {
		DataTable,
		DataTableSkeleton,
		Pagination,
		PaginationSkeleton,
		SkeletonText,
		Toolbar,
		ToolbarContent
	} from 'carbon-components-svelte';

	let selectedRowIds: ReadonlyArray<string> = [];
	let currentPage = 1;
	let pageSize = 25;
	const tableHeaders = [
		{ key: 'institutionId', value: $i18n.t('institutionconnections:table.header.institution') },
		{ key: 'accounts', value: $i18n.t('institutionconnections:table.header.accounts') }
	];
	const tableTitle = $i18n.t('institutionconnections:table.title');

	const listQuery = createQuery({
		...queries.InstitutionConnectionQuery.list({
			skip: (currentPage - 1) * pageSize,
			limit: pageSize
		})
	});

	let deleteMutation: CreateMutationResult<DeleteResponse, Error, undefined>;
	let institutionData: CreateQueryResult<Array<Institution>, Error>;
	let institutionMap: Record<string, Institution> = {};
	let deleteButtonDisabled = true;
	$: {
		const institutionIds =
			$listQuery.data?.items
				.map((item) => item.institutionId)
				.filter((value, index, array) => array.indexOf(value) === index) ?? [];

		institutionData = createQuery({
			...institutionQuery.getMany({ id: institutionIds }),
			enabled: institutionIds.length > 0
		});

		deleteMutation = createMutation({
			...queries.InstitutionConnectionMutation.deleteMany({ ids: selectedRowIds.concat() }),
			onSettled: () => $listQuery.refetch()
		});

		deleteButtonDisabled = selectedRowIds.length == 0 || $deleteMutation?.isLoading;
		$institutionData.data?.reduce((map, institution) => {
			map[institution.id] = institution;
			return map;
		}, institutionMap);
	}
</script>

<PageTitle title="Bank accounts" />

{#if $listQuery.error}
	<LocalError error={$listQuery.error} />
{/if}

{#if $institutionData?.error}
	<LocalError error={$institutionData?.error} />
{/if}

{#if $listQuery.isLoading}
	<DataTableSkeleton headers={tableHeaders} rows={3} />
{:else}
	<DataTable
		headers={tableHeaders}
		rows={$listQuery.data?.items}
		title={tableTitle}
		description={$i18n.t('institutionconnections:table.description') || ''}
		selectable
		batchSelection
		bind:selectedRowIds
	>
		<Toolbar>
			<ToolbarContent>
				<DeleteButton
					isLoading={$deleteMutation.isLoading}
					disabled={deleteButtonDisabled}
					on:click={() => $deleteMutation.mutate(undefined)}
				/>
				<AddButton />
			</ToolbarContent>
		</Toolbar>

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
	</DataTable>
{/if}

{#if $listQuery.isLoading}
	<PaginationSkeleton />
{:else}
	<Pagination
		pageSizes={[pageSize]}
		totalItems={$listQuery.data?.itemsTotal}
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

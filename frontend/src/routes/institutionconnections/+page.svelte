<script lang="ts">
	import type {
		DeleteManyRequest,
		DeleteResponse,
		Institution,
		ListResponseOfInstitutionConnection
	} from '@/api/generated';
	import { queries } from '@/api/queries';
	import LocalError from '@/components/LocalError.svelte';
	import PageBreadcrumbs from '@/components/PageBreadcrumbs.svelte';
	import DeleteButton from '@/components/PaginatedTable/DeleteButton.svelte';
	import PaginatedTable from '@/components/PaginatedTable/PaginatedTable.svelte';
	import { i18n } from '@/services/i18n';
	import { createMutation, createQuery, type CreateQueryResult } from '@tanstack/svelte-query';
	import { SkeletonText } from 'carbon-components-svelte';
	import RefreshButton from './RefreshButton.svelte';

	type InstitutionMap = { [key: string]: Institution };

	const tableHeaders = [
		{ key: 'institutionId', value: $i18n.t('institutionconnections:list.header.institution') },
		{ key: 'accounts', value: $i18n.t('institutionconnections:list.header.accounts') },
		{ key: 'actions', value: $i18n.t('institutionconnections:list.header.actions') }
	];

	let listQuery: CreateQueryResult<ListResponseOfInstitutionConnection, Error>;
	let selectedRowIds: ReadonlyArray<string> = [];

	const deleteMutation = createMutation<DeleteResponse, Error, DeleteManyRequest, unknown>({
		...queries.institutionConnection.deleteMany(),
		onSuccess: () => {
			$listQuery.refetch();
		}
	});

	$: institutionData = createQuery({
		...queries.institution.getMany({
			id: $listQuery?.data?.items
				.map((item) => item.institutionId)
				.filter((item, index, array) => array.indexOf(item) === index)
		}),
		enabled: !!$listQuery?.data?.items.length
	});

	const handleDelete = (ids: string[]) => {
		$deleteMutation.mutate({ ids });
	};

	const getInstitutionByConnectionId = (id: string): Institution | undefined => {
		const connection = $listQuery?.data?.items.find((item) => item.id === id);
		if (!connection) {
			return undefined;
		}

		return $institutionData?.data?.find((item) => item.id === connection.institutionId);
	};
</script>

<PageBreadcrumbs title={$i18n.t('institutionconnections:list.title')} />

{#if $institutionData?.error}
	<LocalError error={$institutionData?.error} />
{/if}

<PaginatedTable
	headers={tableHeaders}
	title={$i18n.t('institutionconnections:list.title')}
	description={$i18n.t('institutionconnections:list.description')}
	listQueryFunction={queries.institutionConnection.list}
	deleteManyMutationFunction={queries.institutionConnection.deleteMany}
	createRoute="/institutionconnections/create"
	batchSelection
	bind:selectedRowIds
	bind:listQuery
>
	<svelte:fragment slot="toolbarButtons">
		<DeleteButton
			title={$i18n.t('institutionconnections:list.actions.delete.title')}
			confirmation={$i18n.t('institutionconnections:list.actions.delete.confirmation', {
				institution: selectedRowIds.map((id) => getInstitutionByConnectionId(id)?.name).join(', ')
			})}
			on:delete={() => handleDelete(selectedRowIds.concat())}
		/>
	</svelte:fragment>
	<svelte:fragment slot="cell" let:row let:cell>
		{@const institution = getInstitutionByConnectionId(row.id)}

		{#if cell.key === 'institutionId'}
			{#if $institutionData.isLoading}
				<SkeletonText />
			{:else}
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
		{:else if cell.key === 'actions'}
			{#if $institutionData.isLoading}
				<SkeletonText />
			{:else}
				<RefreshButton
					institutionConnection={row}
					on:refresh={() => {
						$listQuery.refetch();
					}}
				/>
				<DeleteButton
					iconOnly
					title={$i18n.t('institutionconnections:list.actions.delete.title')}
					confirmation={$i18n.t('institutionconnections:list.actions.delete.confirmation', {
						institution: institution?.name
					})}
					on:delete={() => handleDelete([row.id])}
				/>
			{/if}
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

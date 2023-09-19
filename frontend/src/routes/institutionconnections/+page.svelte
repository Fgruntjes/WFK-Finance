<script lang="ts">
	import LocalError from '@/components/LocalError.svelte';
	import PageBreadcrumbs from '@/components/PageBreadcrumbs.svelte';
	import DeleteButton from '@/components/DeleteButton.svelte';
	import { i18n } from '@/services/i18n';
	import { SkeletonText } from 'carbon-components-svelte';
	import RefreshButton from './RefreshButton.svelte';
	import AddButton from '@/components/AddButton.svelte';
	import {
		DataTable,
		DataTableSkeleton,
		Pagination,
		PaginationSkeleton,
		Toolbar,
		ToolbarContent
	} from 'carbon-components-svelte';
	import type { PageData, InstitutionConnectionsVariables } from './$houdini';
	import type { DataTableRow } from 'carbon-components-svelte/types/DataTable/DataTable.svelte';

	const tableHeaders = [
		{ key: 'institutionId', value: $i18n.t('institutionconnections:list.header.institution') },
		{ key: 'accounts', value: $i18n.t('institutionconnections:list.header.accounts') },
		{ key: 'actions', value: $i18n.t('institutionconnections:list.header.actions') }
	];

	export let data: PageData;

	let selectedRowIds: ReadonlyArray<string> = [];
	let page: number = 1;
	let pageSize: number = 25;
	let tableData: ReadonlyArray<DataTableRow> = [];

	const handleDelete = (ids: string[]) => {};

	const getInstitutionByConnectionId = (id: string): undefined => {
		return undefined;
	};

	export const _InstitutionConnectionsVariables: InstitutionConnectionsVariables = () => {
		return {
			limit: pageSize,
			offset: (page - 1) * pageSize
		};
	};

	$: ({ InstitutionConnections } = data);
	$: {
		const data =
			$InstitutionConnections.data?.institutionConnection?.list?.items?.filter(
				(e) => !!e?.externalId
			) || [];

		tableData = data as unknown as DataTableRow[];
	}
</script>

<PageBreadcrumbs title={$i18n.t('institutionconnections:list.title')} />

{#if $InstitutionConnections.errors}
	{#each $InstitutionConnections.errors as error}
		<LocalError {error} />
	{/each}
{:else if $InstitutionConnections.fetching}
	<DataTableSkeleton headers={tableHeaders} rows={3} />
	<PaginationSkeleton />
{:else}
	<DataTable
		bind:selectedRowIds
		batchSelection
		rows={tableData}
		headers={tableHeaders}
		title={$i18n.t('institutionconnections:list.title')}
		description={$i18n.t('institutionconnections:list.description')}
	>
		<Toolbar>
			<ToolbarContent>
				<AddButton route="/institutionconnections/create" />
				<DeleteButton
					title={$i18n.t('institutionconnections:list.actions.delete.title')}
					confirmation={$i18n.t('institutionconnections:list.actions.delete.confirmation', {
						institution: selectedRowIds
							.map((id) => getInstitutionByConnectionId(id)?.name)
							.join(', ')
					})}
					on:delete={() => handleDelete(selectedRowIds.concat())}
				/>
			</ToolbarContent>
		</Toolbar>

		<svelte:fragment slot="cell" let:row let:cell>
			{@const institution = getInstitutionByConnectionId(row.id)}

			{#if cell.key === 'institutionId'}
				{#if true}
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
				{#if true}
					<SkeletonText />
				{:else}
					<RefreshButton
						institutionConnection={row}
						on:refresh={() => {
							// TODO refresh
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
	</DataTable>

	<Pagination pageSizes={[25]} totalItems={30} bind:page bind:pageSize />
{/if}

<style lang="scss">
	.institution-logo {
		border-radius: 10%;
		max-height: 2em;
		max-width: 2em;
	}
</style>

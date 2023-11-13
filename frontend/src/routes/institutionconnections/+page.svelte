<script lang="ts">
	import LocalError from '@/components/LocalError.svelte';
	import PageBreadcrumbs from '@/components/PageBreadcrumbs.svelte';
	import DeleteButton from '@/components/DeleteButton.svelte';
	import { i18n } from '@/services/i18n';
	import RefreshButton from './+page/RefreshButton.svelte';
	import AddButton from '@/components/AddButton.svelte';
	import {
		DataTable,
		DataTableSkeleton,
		Pagination,
		PaginationSkeleton,
		Toolbar,
		ToolbarContent
	} from 'carbon-components-svelte';
	import { graphql } from '$houdini';
	import { onMount } from 'svelte';

	const tableHeaders = [
		{ key: 'institutionId', value: $i18n.t('institutionconnections:list.header.institution') },
		{ key: 'accounts', value: $i18n.t('institutionconnections:list.header.accounts') },
		{ key: 'actions', value: $i18n.t('institutionconnections:list.header.actions') }
	];

	let selectedRowIds: ReadonlyArray<string> = [];
	let page = 1;
	let pageSize = 25;

	// TODO: If somebody is in the mood, please change to using fragments here. Sorry I am tired.
	const data = graphql(`
		query InstitutionConnections($offset: Int!, $limit: Int!) {
			institutionConnection {
				list(offset: $offset, limit: $limit) {
					items {
						id
						externalId
						accounts {
							iban
						}
						institution {
							id
							name
							logo
						}
					}
					totalCount
				}
			}
		}
	`);

	const refreshMutation = graphql(`
		mutation RefreshInstitutionConnection($Id: Guid!) {
			institutionConnection {
				refreshId(id: $Id) {
					id
					externalId
					accounts {
						iban
					}
					institution {
						id
						name
						logo
					}
				}
			}
		}
	`);

	const loadData = async () => {
		await data.fetch({
			variables: {
				offset: (page - 1) * pageSize,
				limit: pageSize
			}
		});
	};
	onMount(loadData);
</script>

<PageBreadcrumbs title={$i18n.t('institutionconnections:list.title')} />
{#if $data.errors}
	{#each $data.errors as error}
		<LocalError {error} />
	{/each}
{:else if $data.fetching || !$data.data}
	<DataTableSkeleton headers={tableHeaders} rows={3} />
	<PaginationSkeleton />
{:else}
	<DataTable
		bind:selectedRowIds
		batchSelection
		rows={$data.data?.institutionConnection?.list?.items || []}
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
						institution: selectedRowIds.join(', ')
					})}
				/>
			</ToolbarContent>
		</Toolbar>

		<svelte:fragment slot="cell" let:row let:cell>
			{@const institution = row.institution}
			{#if cell.key === 'institutionId'}
				{#if institution?.logo}
					<img alt={institution.name} src={institution.logo} class="institution-logo" />
				{/if}

				{institution?.name}
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
				<RefreshButton
					isLoading={$refreshMutation.fetching}
					on:click={() => {
						refreshMutation.mutate({
							Id: row.id
						});
					}}
				/>
				<DeleteButton
					iconOnly
					title={$i18n.t('institutionconnections:list.actions.delete.title')}
					confirmation={$i18n.t('institutionconnections:list.actions.delete.confirmation', {
						institution: institution?.name
					})}
				/>
			{:else}
				{cell.value}
			{/if}
		</svelte:fragment>
	</DataTable>

	<Pagination
		pageSizes={[25]}
		totalItems={$data.data?.institutionConnection?.list?.totalCount}
		bind:page
		bind:pageSize
		on:change={loadData}
	/>
{/if}

<style lang="scss">
	.institution-logo {
		border-radius: 10%;
		max-height: 2em;
		max-width: 2em;
	}
</style>

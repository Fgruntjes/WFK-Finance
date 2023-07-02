<script lang="ts">
	import type { ListQueryFunction } from './types';

	import AddButton from './AddButton.svelte';

	import { createQuery, type QueryKey } from '@tanstack/svelte-query';
	import {
		DataTable,
		DataTableSkeleton,
		Pagination,
		PaginationSkeleton,
		Toolbar,
		ToolbarContent
	} from 'carbon-components-svelte';

	import LocalError from '@/components/LocalError.svelte';
	import type {
		DataTableCell,
		DataTableHeader,
		DataTableRow
	} from 'carbon-components-svelte/types/DataTable/DataTable.svelte';

	type TEntity = $$Generic<DataTableRow>;
	type TQueryFunctionData = $$Generic;
	type TQueryKey = $$Generic<QueryKey>;
	type TDeleteMutationResponse = $$Generic;

	interface $$Slots {
		cell: {
			row: DataTableRow;
			cell: DataTableCell;
		};
		toolbarButtons: {};
	}

	export let pageSize = 25;
	export let headers: ReadonlyArray<DataTableHeader>;
	export let createRoute: string | undefined = undefined;
	export let listQueryFunction: ListQueryFunction<TQueryFunctionData, TEntity, TQueryKey>;
	export let currentPage = 1;
	export let selectedRowIds: ReadonlyArray<string> = [];

	export const listQuery = createQuery({
		...listQueryFunction({
			skip: (currentPage - 1) * pageSize,
			limit: pageSize
		})
	});
</script>

{#if $listQuery.error}
	<LocalError error={$listQuery.error} />
{/if}

{#if $listQuery.isLoading}
	<DataTableSkeleton {headers} rows={3} />
{:else}
	<DataTable bind:selectedRowIds rows={$listQuery.data?.items} {headers} {...$$restProps}>
		{#if createRoute}
			<Toolbar>
				<ToolbarContent>
					{#if createRoute}
						<AddButton route={createRoute} />
					{/if}
					<slot name="toolbarButtons" />
				</ToolbarContent>
			</Toolbar>
		{/if}

		<svelte:fragment slot="cell" let:row let:cell>
			<slot name="cell" {row} {cell} />
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

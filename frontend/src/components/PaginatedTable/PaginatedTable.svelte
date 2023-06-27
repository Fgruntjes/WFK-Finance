<script lang="ts" context="module">
	import type { CreateQueryOptions, QueryKey } from '@tanstack/svelte-query';

	type ListRequest = {
		skip: number;
		limit: number;
	};

	type ListResult<TEntity> = {
		items: Array<TEntity>;
		itemsTotal: number;
	};

	type ListActionType<TQueryFnData, TEntity, TQueryKey extends QueryKey> = (
		request: ListRequest
	) => CreateQueryOptions<TQueryFnData, Error, ListResult<TEntity>, TQueryKey>;
</script>

<script lang="ts">
	import { createQuery } from '@tanstack/svelte-query';
	import {
		DataTable,
		DataTableSkeleton,
		Pagination,
		PaginationSkeleton
	} from 'carbon-components-svelte';
	import type { ComponentProps } from 'svelte';

	import LocalError from '@/components/LocalError.svelte';
	import type {
		DataTableCell,
		DataTableHeader,
		DataTableRow
	} from 'carbon-components-svelte/types/DataTable/DataTable.svelte';

	type TEntity = $$Generic<DataTableRow>;
	type TQueryFnData = $$Generic;
	type TQueryKey = $$Generic<QueryKey>;

	interface $$Props extends ComponentProps<DataTable> {
		pageSize?: number;
		headers: ReadonlyArray<DataTableHeader>;
		listAction: ListActionType<TQueryFnData, TEntity, TQueryKey>;
	}

	interface $$Slots {
		cell: {
			row: DataTableRow;
			cell: DataTableCell;
		};
	}

	export let pageSize = 25;
	export let headers: ReadonlyArray<DataTableHeader>;
	export let listAction: ListActionType<TQueryFnData, TEntity, TQueryKey>;
	let currentPage = 1;

	export const listQuery = createQuery({
		...listAction({
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
	<DataTable rows={$listQuery.data?.items} {headers} {...$$restProps}>
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

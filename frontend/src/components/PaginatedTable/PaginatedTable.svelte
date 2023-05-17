<script lang="ts">
	import { createServiceQuery } from '@/api/createServiceQuery';
	import type { Configuration } from '@/api/generated';
	import Loader from '@/components/Loader.svelte';
	import QueryErrorContainer from '@/components/QueryErrorContainer.svelte';
	import {
		Pagination,
		PaginationSkeleton,
		StructuredList,
		StructuredListBody
	} from 'carbon-components-svelte';
	import type { StructuredListProps } from 'carbon-components-svelte/types/StructuredList/StructuredList.svelte';
	import type { ListService } from './ListService';

	export let queryKey: string | string[];
	export let listProps: StructuredListProps = { condensed: true };
	export let pageSize = 10;
	export let pageSizes = [pageSize];

	let currentPage: 1;

	type TData = $$Generic<TData>;
	export let serviceType: new (configuration: Configuration) => ListService<TData>;

	const query = createServiceQuery(serviceType, {
		queryFn: (service) =>
			service.list({
				skip: (currentPage - 1) * pageSize,
				limit: pageSize
			}),
		queryKey: typeof queryKey === 'string' ? ['default', queryKey, 'list'] : queryKey
	});
</script>

<QueryErrorContainer {query}>
	<StructuredList {...listProps}>
		<slot name="head" />
		<StructuredListBody>
			{#if $query.isLoading}
				<Loader />
			{:else}
				{#each $query.data?.items || [] as item}
					<slot name="item" {item} />
				{/each}
			{/if}
		</StructuredListBody>
	</StructuredList>
	{#if $query.isLoading}
		<PaginationSkeleton />
	{:else}
		<Pagination
			totalItems={$query.data?.itemsTotal}
			bind:page={currentPage}
			{pageSize}
			{pageSizes}
		/>
	{/if}
</QueryErrorContainer>

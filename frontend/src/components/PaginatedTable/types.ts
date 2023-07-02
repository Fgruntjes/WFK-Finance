import type { CreateQueryOptions, QueryKey } from '@tanstack/svelte-query';

export type ListRequest = {
    skip: number;
    limit: number;
};

export type ListResult<TEntity> = {
    items: Array<TEntity>;
    itemsTotal: number;
};

export type ListQueryFunction<TQueryFnData, TEntity, TQueryKey extends QueryKey> = (
    request: ListRequest
) => CreateQueryOptions<TQueryFnData, Error, ListResult<TEntity>, TQueryKey>;

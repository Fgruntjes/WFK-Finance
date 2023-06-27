import type {
	CreateQueryOptions,
	CreateQueryResult,
	QueryFunctionContext,
	QueryKey
} from '@tanstack/svelte-query';
import { createQuery } from '@tanstack/svelte-query';
import { createService } from './createService';
import type { Configuration } from './generated';

export type QueryFunction<TService, T = unknown, TQueryKey extends QueryKey = QueryKey> = (
	service: TService,
	context: QueryFunctionContext<TQueryKey>
) => T | Promise<T>;

export function createServiceQuery<
	TService,
	TQueryFnData = unknown,
	TData = TQueryFnData,
	TQueryKey extends QueryKey = QueryKey
>(
	serviceType: new (configuration: Configuration) => TService,
	queryOptions: Omit<
		Omit<CreateQueryOptions<TQueryFnData, Error, TData, TQueryKey>, 'initialData'>,
		'queryFn'
	> & {
		queryFn: QueryFunction<TService, TQueryFnData, TQueryKey>;
		initialData?: () => undefined;
	}
): CreateQueryResult<TData, Error> {
	const service = createService(serviceType);
	const queryResult = createQuery({
		...queryOptions,
		queryFn: async (context) => queryOptions.queryFn(service, context)
	});

	return queryResult;
}

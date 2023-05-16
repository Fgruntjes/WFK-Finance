import { ErrorType } from '@components/ErrorElement';
import { QueryFunctionContext, QueryKey, UseQueryOptions, UseQueryResult, useQuery } from '@tanstack/react-query';
import { BaseAPI, Configuration } from "./generated";
import useConfiguration from './useConfiguration';

export type QueryFunction<
    TService extends BaseAPI,
    T = unknown,
    TQueryKey extends QueryKey = QueryKey,
> = (service: TService, context: QueryFunctionContext<TQueryKey>) => T | Promise<T>

function useServiceQuery<
    TService extends BaseAPI,
    TQueryFnData = unknown,
    TData = TQueryFnData,
    TQueryKey extends QueryKey = QueryKey,
>(
    serviceType: new (configuration: Configuration) => TService,
    queryOptions: 
        Omit<Omit<UseQueryOptions<TQueryFnData, ErrorType, TData, TQueryKey>, 'initialData'>, 'queryFn'>
        & {
            queryFn: QueryFunction<TService, TQueryFnData, TQueryKey>,
            initialData?: () => undefined
        },
): UseQueryResult<TData, ErrorType> {
    const configuration = useConfiguration();
    const service = new serviceType(configuration);
    return useQuery({
        ...queryOptions,
        queryFn: (context) => queryOptions.queryFn(service, context)
    })
}

export default useServiceQuery;

import { QueryFunctionContext, QueryKey, UseQueryOptions, UseQueryResult, useQuery } from '@tanstack/react-query';
import { Configuration } from "./generated";
import useConfiguration from './useConfiguration';

export type QueryFunction<
    TService,
    T = unknown,
    TQueryKey extends QueryKey = QueryKey,
> = (service: TService, context: QueryFunctionContext<TQueryKey>) => T | Promise<T>

function useServiceQuery<
    TService,
    TQueryFnData = unknown,
    TData = TQueryFnData,
    TQueryKey extends QueryKey = QueryKey,
>(
    serviceType: new (configuration: Configuration) => TService,
    queryOptions: 
        Omit<Omit<UseQueryOptions<TQueryFnData, Error, TData, TQueryKey>, 'initialData'>, 'queryFn'>
        & {
            queryFn: QueryFunction<TService, TQueryFnData, TQueryKey>,
            initialData?: () => undefined
        },
): UseQueryResult<TData, Error> {
    const configuration = useConfiguration();
    const service = new serviceType(configuration);
    const queryResult = useQuery({
        ...queryOptions,
        queryFn: (context) => queryOptions.queryFn(service, context)
    });

    return queryResult;
}

export default useServiceQuery;
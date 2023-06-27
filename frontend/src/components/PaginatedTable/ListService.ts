import type { InitOverrideFunction } from '@/api/generated';
import type { ListRequest } from './ListRequest';
import type { ListResult } from './ListResponse';

export interface ListService<TData> {
	list: (
		requestParameters?: ListRequest,
		initOverrides?: RequestInit | InitOverrideFunction
	) => Promise<ListResult<TData>>;
}

import { createQueryKeys } from '@lukemorales/query-key-factory';
import { createService } from '../createService';
import { InstitutionApi, type GetManyRequest, type SearchRequest } from '../generated';

const service = createService(InstitutionApi);

export const institutionQuery = createQueryKeys('institution', {
	getMany: (request: GetManyRequest) => ({
		queryKey: ['getMany', { request }],
		queryFn: () => service.getMany(request.id)
	}),
	search: (request: SearchRequest) => ({
		queryKey: ['search', { request }],
		queryFn: () => service.search(request.country)
	})
});

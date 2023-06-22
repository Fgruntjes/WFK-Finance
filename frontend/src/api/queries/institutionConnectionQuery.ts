import { createQueryKeys } from '@lukemorales/query-key-factory';
import { createService } from '../createService';
import { InstitutionConnectionApi, type ListRequest } from '../generated';

const service = createService(InstitutionConnectionApi);

export const institutionConnectionQuery = createQueryKeys('InstitutionConnectionApi', {
    list: (request: ListRequest) => ({
        queryKey: [{ request }],
        queryFn: () => service.list(request),
    }),
})
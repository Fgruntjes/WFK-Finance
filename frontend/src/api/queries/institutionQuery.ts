import { createQueryKeys } from '@lukemorales/query-key-factory';
import { createService } from '../createService';
import { InstitutionApi, type GetManyRequest } from '../generated';

const service = createService(InstitutionApi);

export const institutionQuery = createQueryKeys('InstitutionApi', {
    getMany: (request: GetManyRequest) => ({
        queryKey: [{ request }],
        queryFn: () => service.getMany(request),
    }),
})


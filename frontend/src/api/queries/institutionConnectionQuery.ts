import { createMutationKeys, createQueryKeys } from '@lukemorales/query-key-factory';
import { createService } from '../createService';
import { InstitutionConnectionApi, type DeleteManyRequest, type ListRequest } from '../generated';

const service = createService(InstitutionConnectionApi);

export const institutionConnectionQuery = createQueryKeys('InstitutionConnectionQuery', {
    list: (request: ListRequest) => ({
        queryKey: [{ request }],
        queryFn: () => service.list(request),
    }),
})

export const institutionConnectionMutation = createMutationKeys('InstitutionConnectionMutation', {
    deleteMany: (request: DeleteManyRequest) => ({
        mutationKey: [{ request }],
        mutationFn: () => service.deleteMany(request),
    }),
})
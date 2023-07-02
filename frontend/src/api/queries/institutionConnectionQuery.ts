import { createMutationKeys, createQueryKeys } from '@lukemorales/query-key-factory';
import { createService } from '../createService';
import {
    InstitutionConnectionApi,
    type DeleteManyRequest,
    type InstitutionConnectionCreateRequest,
    type InstitutionConnectionRefreshByExternalIdRequest,
    type InstitutionConnectionRefreshRequest,
    type ListRequest
} from '../generated';

const service = createService(InstitutionConnectionApi);

export const institutionConnectionQuery = createQueryKeys('institutionConnection', {
    list: (request: ListRequest) => ({
        queryKey: ['list', { ...request }],
        queryFn: () => service.list(request.skip, request.limit),
    }),
    refresh: (request: InstitutionConnectionRefreshRequest | InstitutionConnectionRefreshByExternalIdRequest) => ({
        queryKey: ['refresh', { ...request }],
        queryFn: () => {
            if ('externalId' in request) {
                return service.refreshByExternalId(request)
            }
            return service.refresh(request)
        },
    }),
})

export const institutionConnectionMutation = createMutationKeys('institutionConnection', {
    deleteMany: () => ({
        mutationKey: ['deleteMany'],
        mutationFn: (request: DeleteManyRequest) => service.deleteMany(request.ids),
    }),
    create: () => ({
        mutationKey: ['create'],
        mutationFn: (request: InstitutionConnectionCreateRequest) => service.create(request),
    }),
})
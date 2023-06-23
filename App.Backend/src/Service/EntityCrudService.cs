using App.Backend.Auth;
using App.Backend.Data.Entity;
using App.Backend.DTO;
using MongoDB.Bson;
using MongoDB.Driver;

namespace App.Backend.Service;

public class EntityCrudService<TEntity> where TEntity : IEntity
{
    private readonly AuthContext _authContext;
    private readonly IMongoCollection<TEntity> _mongoCollection;

    public EntityCrudService(AuthContext authContext, IMongoCollection<TEntity> mongoCollection)
    {
        _authContext = authContext;
        _mongoCollection = mongoCollection;
    }

    public async Task<ListResponse<TEntity>> List(ListRequest request, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter
            .Eq(y => y.TenantId, await GetTenantId());

        var countFacet = AggregateFacet.Create(
                        "count",
                        PipelineDefinition<TEntity, AggregateCountResult>.Create(new[]
                        {
                PipelineStageDefinitionBuilder.Count<TEntity>()
                        }));
        var dataFacet = AggregateFacet.Create("data",
            PipelineDefinition<TEntity, TEntity>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Skip<TEntity>(request.Skip),
                PipelineStageDefinitionBuilder.Limit<TEntity>(request.Limit),
            }));

        var aggregation = await _mongoCollection.Aggregate()
            .Match(filter)
            .Facet(countFacet, dataFacet)
            .ToListAsync();

        var itemsTotal = aggregation.First()
            .Facets.First(x => x.Name == "count")
            .Output<AggregateCountResult>()
            .First()
            .Count;

        var items = aggregation.First()
            .Facets.First(x => x.Name == "data")
            .Output<TEntity>();

        return new ListResponse<TEntity>(items.ToArray(), itemsTotal);
    }

    public async Task<DeleteResponse> Delete(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = await GetTenantId(cancellationToken);
        var filter = Builders<TEntity>.Filter.And(
            Builders<TEntity>.Filter.Eq(c => c.TenantId, tenantId),
            Builders<TEntity>.Filter.In(c => c.Id, request.Ids.Select(x => new ObjectId(x))));

        var result = await _mongoCollection
            .DeleteManyAsync(filter, cancellationToken: cancellationToken);

        return new DeleteResponse(result.DeletedCount);
    }

    protected async Task<ObjectId> GetTenantId(CancellationToken cancellationToken = default)
    {
        return new ObjectId((await _authContext.GetTenant(cancellationToken)).Id);
    }
}
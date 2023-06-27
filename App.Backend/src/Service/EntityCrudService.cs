using App.Backend.Auth;
using App.Backend.Data;
using App.Backend.Data.Entity;
using App.Backend.DTO;
using MongoDB.Bson;
using MongoDB.Driver;

namespace App.Backend.Service;

public class EntityCrudService<TEntity> where TEntity : IEntity
{
    private readonly AuthContext _authContext;
    private readonly DatabaseContext _databaseContext;
    private IMongoCollection<TEntity> MongoCollection => _databaseContext.GetCollection<TEntity>();

    public EntityCrudService(AuthContext authContext, DatabaseContext databaseContext)
    {
        _authContext = authContext;
        _databaseContext = databaseContext;
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

        var aggregation = await MongoCollection.Aggregate()
            .Match(filter)
            .Facet(countFacet, dataFacet)
            .ToListAsync();
        var resultFacets = aggregation.First().Facets;
        var countResult = resultFacets.First(x => x.Name == "count")
            .Output<AggregateCountResult>();

        if (aggregation.Count == 0 || countResult.Count == 0)
        {
            return new ListResponse<TEntity>(new TEntity[0], 0);
        }

        var itemsTotal = countResult.First().Count;
        var items = resultFacets.First(x => x.Name == "data")
            .Output<TEntity>();

        return new ListResponse<TEntity>(items.ToArray(), itemsTotal);
    }

    public async Task<DeleteResponse> DeleteMany(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        var ids = request.Ids.Where(x => !String.IsNullOrEmpty(x)).ToArray();
        if (ids.Length == 0)
        {
            throw new ArgumentException("Ids must not be empty", nameof(request.Ids));
        }

        var tenantId = await GetTenantId(cancellationToken);
        var filter = Builders<TEntity>.Filter.And(
            Builders<TEntity>.Filter.Eq(c => c.TenantId, tenantId),
            Builders<TEntity>.Filter.In(c => c.Id, ids.Select(x => new ObjectId(x))));

        var result = await MongoCollection
            .DeleteManyAsync(filter, cancellationToken: cancellationToken);

        return new DeleteResponse(result.DeletedCount);
    }

    protected async Task<ObjectId> GetTenantId(CancellationToken cancellationToken = default)
    {
        return new ObjectId((await _authContext.GetTenant(cancellationToken)).Id);
    }
}
using App.Backend.Auth;
using App.Backend.Data.Entity;
using App.Backend.DTO;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace App.Backend.Controllers;

[Produces("application/json")]
public abstract class AbstractListController<TEntity, TDto> : ControllerBase
    where TEntity : IListableEntity
    where TDto : IEntityConvertable<TEntity, TDto>
{
    private readonly AuthContext _authContext;
    private readonly IMongoCollection<TEntity> _mongoCollection;

    public AbstractListController(AuthContext authContext, IMongoCollection<TEntity> mongoCollection)
    {
        _authContext = authContext;
        _mongoCollection = mongoCollection;
    }

    [HttpGet()]
    public async Task<ActionResult<ListResponse<TDto>>> List([FromQuery] ListRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = await _authContext.GetTenant();
        var filter = Builders<TEntity>.Filter
            .Eq(y => y.TenantId, new ObjectId(tenant.Id));

        try
        {
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
                .Output<TEntity>()
                .Select(TDto.FromEntity);

            return new ListResponse<TDto>(items.ToArray(), itemsTotal);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}

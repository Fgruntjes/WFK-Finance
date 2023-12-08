using App.Backend.GraphQL.Controllers;
using App.Backend.GraphQL.Type;
using App.Lib.InstitutionConnection.Exception;
using App.Lib.InstitutionConnection.Service;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace App.Backend.Controllers;

[GraphRoute("institutionConnection")]
public class InstitutionConnectionRefreshController : GraphController
{
    private readonly IInstitutionConnectionRefreshService _refreshService;

    public InstitutionConnectionRefreshController(IInstitutionConnectionRefreshService refreshService)
    {
        _refreshService = refreshService;
    }

    [Authorize]
    [Mutation("refreshExternalId", typeof(InstitutionConnection))]
    public async Task<IGraphActionResult> Refresh(string externalId, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _refreshService.Refresh(externalId, cancellationToken);
            return Ok(entity.ToGraphQLType());
        }
        catch (InstitutionConnectionNotFoundException exception)
        {
            return new BadRequestGraphQlActionResult(exception);
        }
    }

    [Authorize]
    [Mutation("refreshId", typeof(InstitutionConnection))]
    public async Task<IGraphActionResult> Refresh(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _refreshService.Refresh(id, cancellationToken);
            return Ok(entity.ToGraphQLType());
        }
        catch (InstitutionConnectionNotFoundException exception)
        {
            return new BadRequestGraphQlActionResult(exception);
        }
    }
}
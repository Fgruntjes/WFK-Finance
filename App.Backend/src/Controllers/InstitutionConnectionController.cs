using App.Backend.Auth;
using App.Backend.Data;
using App.Backend.Data.Entity;
using App.Backend.DTO;
using App.Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace App.Backend.Controllers;

[Tags("InstitutionConnection")]
[Produces("application/json")]
[Authorize("bankacounts:read")]
[Route("/InstitutionConnection")]
public class InstitutionConnectionController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;
    private readonly AuthContext _authContext;
    private readonly InstitutionConnectionService _institutionConnectionService;
    private readonly EntityCrudService<InstitutionConnectionEntity> _entityCrudService;

    public InstitutionConnectionController(
        InstitutionConnectionService institutionConnectionService,
        DatabaseContext databaseContext,
        EntityCrudService<InstitutionConnectionEntity> entityCrudService,
        AuthContext authContext)
    {
        _databaseContext = databaseContext;
        _authContext = authContext;
        _institutionConnectionService = institutionConnectionService;
        _entityCrudService = entityCrudService;
    }

    [HttpGet()]
    public async Task<ActionResult<ListResponse<InstitutionConnection>>> List([FromQuery] ListRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _entityCrudService.List(request);
        return new ListResponse<InstitutionConnection>(
            result.Items.Select(InstitutionConnection.FromEntity).ToArray(),
            result.ItemsTotal);
    }

    [HttpDelete()]
    public async Task<ActionResult<DeleteResponse>> DeleteMany([FromQuery] DeleteRequest request, CancellationToken cancellationToken = default)
    {
        return await _entityCrudService.DeleteMany(request);
    }

    [HttpPost()]
    public async Task<ActionResult<InstitutionConnection>> Create([FromBody] InstitutionConnectionCreateRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _institutionConnectionService.Create(new ObjectId(request.InstitutionId), request.ReturnUri);
        return InstitutionConnection.FromEntity(result);
    }

    [HttpPost()]
    [Route("/InstitutionConnection/Refresh")]
    public async Task<InstitutionConnection> Refresh([FromBody] InstitutionConnectionRefreshRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _institutionConnectionService.Refresh(new ObjectId(request.Id));
        return InstitutionConnection.FromEntity(result);
    }

    [HttpPost()]
    [Route("/InstitutionConnection/RefreshByExternalId")]
    public async Task<InstitutionConnection> RefreshByExternalId([FromBody] InstitutionConnectionRefreshByExternalIdRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _institutionConnectionService.RefreshByExternalId(request.ExternalId);
        return InstitutionConnection.FromEntity(result);
    }
}

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
public class InstitutionConnectionController : AbstractListController<InstitutionConnectionEntity, InstitutionConnection>
{
    private readonly DatabaseContext _databaseContext;
    private readonly AuthContext _authContext;
    private readonly InstitutionConnectionService _institutionConnectionService;

    public InstitutionConnectionController(
        InstitutionConnectionService institutionConnectionService,
        DatabaseContext databaseContext,
        AuthContext authContext) : base(authContext, databaseContext.InstitutionConnections)
    {
        _databaseContext = databaseContext;
        _authContext = authContext;
        _institutionConnectionService = institutionConnectionService;
    }

    [HttpPost()]
    public async Task<InstitutionConnection> Create([FromBody] InstitutionConnectionCreateRequest request, CancellationToken cancellationToken = default)
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
}

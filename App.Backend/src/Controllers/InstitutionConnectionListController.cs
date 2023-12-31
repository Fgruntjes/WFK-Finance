using App.Backend.Dto;
using App.Backend.Mvc;
using App.Lib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using InstitutionConnection = App.Backend.Dto.InstitutionConnection;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(RouteBase)]
public class InstitutionConnectionListController : ControllerBase
{
    public const string RouteBase = "/institutionconnections";
    public const string RouteName = nameof(InstitutionConnectionListController);

    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;

    public InstitutionConnectionListController(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [HttpGet(Name = RouteName)]
    [ProducesResponseType(typeof(ICollection<InstitutionConnection>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] RangeParameter? range,
        CancellationToken cancellationToken = default)
    {
        range ??= new RangeParameter();
        
        var organisationId = _organisationIdProvider.GetOrganisationId();
        var query = _database.InstitutionConnections
            .Where(e => e.OrganisationId == organisationId);

        var totalCount = await query.CountAsync(cancellationToken);
        var result = await query
            .OrderBy(e => e.CreatedAt)
            .Skip(range.Offset)
            .Take(range.Limit)
            .Include(e => e.Accounts)
            .Select(e => e.ToDto())
            .ToListAsync(cancellationToken);

        return new ListResult<InstitutionConnection>(
            result,
            RouteBase,
            range.Start,
            range.End,
            totalCount);
    }
}
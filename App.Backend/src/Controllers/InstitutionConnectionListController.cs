using App.Backend.Dto;
using App.Lib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstitutionConnection = App.Backend.Dto.InstitutionConnection;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(InstitutionConnectionGetController.RouteBase)]
public class InstitutionConnectionListController : ControllerBase
{
    public const string RouteName = nameof(InstitutionConnectionListController);

    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;

    public InstitutionConnectionListController(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [HttpGet(Name = RouteName)]
    [ProducesResponseType(typeof(ListResult<InstitutionConnection>), StatusCodes.Status200OK)]
    public async Task<ListResult<InstitutionConnection>> List(int offset = 0, int limit = 25, CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.GetOrganisationId();
        var query = _database.InstitutionConnections
            .Where(e => e.OrganisationId == organisationId);

        var totalCount = await query.CountAsync(cancellationToken);
        var result = await query
            .OrderBy(e => e.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .Include(e => e.Accounts)
            .Select(e => e.ToDto())
            .ToListAsync(cancellationToken);

        return new ListResult<InstitutionConnection>
        {
            Items = result,
            TotalCount = totalCount
        };
    }
}
using App.Backend.Dto;
using App.Backend.Mvc;
using App.Lib.Data;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    [ProducesResponseType(typeof(ICollection<InstitutionConnectionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] GridifyQuery query,
        CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.GetOrganisationId();
        var result = await _database.InstitutionConnections
            .Where(e => e.OrganisationId == organisationId)
            .GridifyQueryableAsync(query, null, cancellationToken);

        var items = await result.Query
            .Include(e => e.Accounts)
            .Select(e => new InstitutionConnectionDto
            {
                Id = e.Id,
                InstitutionId = e.InstitutionId,
                ExternalId = e.ExternalId,
                ConnectUrl = e.ConnectUrl,
                Accounts = e.Accounts.Select(a => new InstitutionAccountDto
                {
                    Id = a.Id,
                    ExternalId = a.ExternalId,
                    Iban = a.Iban,
                    ImportStatus = a.ImportStatus,
                    LastImport = a.LastImport.HasValue ? a.LastImport.Value.ToDateTimeUtc() : null,
                    TransactionCount = a.Transactions.Count
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        var start = query.Page * query.PageSize;
        return ListResult<InstitutionConnectionDto>.Create(items, RouteBase, query, result.Count);
    }
}
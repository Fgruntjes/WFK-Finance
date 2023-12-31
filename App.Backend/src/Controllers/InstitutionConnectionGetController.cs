using App.Backend.Dto;
using App.Lib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(InstitutionConnectionListController.RouteBase)]
public class InstitutionConnectionGetController : ControllerBase
{
    public const string RouteName = nameof(InstitutionConnectionGetController);

    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;

    public InstitutionConnectionGetController(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [HttpGet("{id:guid}", Name = RouteName)]
    [ProducesResponseType(typeof(InstitutionConnection), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.GetOrganisationId();
        var organisationEntity = await _database.InstitutionConnections
            .Where(e => e.Id == id && e.OrganisationId == organisationId)
            .OrderBy(e => e.CreatedAt)
            .Include(e => e.Accounts)
            .Take(1)
            .SingleOrDefaultAsync(cancellationToken);

        if (organisationEntity == null)
        {
            return NotFound();
        }

        return Ok(organisationEntity.ToDto());
    }
}
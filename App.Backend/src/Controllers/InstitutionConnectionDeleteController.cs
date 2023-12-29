using App.Lib.Data;
using App.Lib.InstitutionConnection.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(InstitutionConnectionGetController.RouteBase)]
public class InstitutionConnectionDeleteController : ControllerBase
{
    public const string RouteName = nameof(InstitutionConnectionDeleteController);

    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;

    public InstitutionConnectionDeleteController(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [HttpDelete(Name = RouteName)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        [FromBody] ICollection<Guid> connectionIds,
        CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.GetOrganisationId();
        var entities = _database.InstitutionConnections
            .Where(e => e.OrganisationId == organisationId)
            .Where(e => connectionIds.Contains(e.Id));
        var deleteCount = await entities.CountAsync(cancellationToken);

        _database.InstitutionConnections.RemoveRange(entities);
        await _database.SaveChangesAsync(cancellationToken);

        return Ok(deleteCount);
    }
}
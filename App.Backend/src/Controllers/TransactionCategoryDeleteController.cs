using App.Lib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(TransactionCategoryListController.RouteBase)]
public class TransactionCategoryDeleteController : ControllerBase
{
    public const string RouteName = nameof(TransactionCategoryDeleteController);

    private readonly DatabaseContext _database;

    private readonly IOrganisationIdProvider _organisationIdProvider;

    public TransactionCategoryDeleteController(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }


    [HttpDelete(Name = RouteName)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        [FromQuery] ICollection<Guid> id,
        CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.GetOrganisationId();
        var entities = _database.TransactionCategory
            .Where(e => e.OrganisationId == organisationId)
            .Where(e => id.Contains(e.Id));
        var deleteCount = await entities.CountAsync(cancellationToken);

        _database.TransactionCategory.RemoveRange(entities);
        await _database.SaveChangesAsync(cancellationToken);

        return Ok(deleteCount);
    }
}
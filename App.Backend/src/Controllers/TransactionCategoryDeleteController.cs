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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        [FromQuery] ICollection<Guid> id,
        CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.GetOrganisationId();
        var entities = _database.TransactionCategory
            .Where(e => e.OrganisationId == organisationId)
            .Where(e => id.Contains(e.Id))
            .Include(e => e.Children);

        foreach (var entity in entities)
        {
            if (entity.Children.Any())
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "An error occurred",
                    Detail = "Cannot delete a category with children.",
                    Status = StatusCodes.Status400BadRequest,
                });
            }
        }

        var deleteCount = await entities.CountAsync(cancellationToken);
        _database.TransactionCategory.RemoveRange(entities);
        await _database.SaveChangesAsync(cancellationToken);

        return Ok(deleteCount);
    }
}
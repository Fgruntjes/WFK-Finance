using App.Backend.Dto;
using App.Lib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(TransactionCategoryListController.RouteBase)]
public class TransactionCategoryGetController : ControllerBase
{
    public const string RouteName = nameof(TransactionCategoryGetController);

    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;

    public TransactionCategoryGetController(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [HttpGet("{id:guid}", Name = RouteName)]
    [ProducesResponseType(typeof(TransactionCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _database.TransactionCategory
            .Where(e => e.OrganisationId == _organisationIdProvider.GetOrganisationId())
            .Where(e => e.Id == id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity.ToDto());
    }
}

using App.Backend.Dto;
using App.Lib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(InstitutionListController.RouteBase)]
public class InstitutionGetController : ControllerBase
{
    public const string RouteName = nameof(InstitutionGetController);

    private readonly DatabaseContext _database;

    public InstitutionGetController(DatabaseContext database)
    {
        _database = database;
    }

    [HttpGet("{id:guid}", Name = RouteName)]
    [ProducesResponseType(typeof(Dto.InstitutionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _database.Institutions
            .Where(e => e.Id == id)
            .OrderBy(e => e.CreatedAt)
            .Take(1)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity.ToDto());
    }
}
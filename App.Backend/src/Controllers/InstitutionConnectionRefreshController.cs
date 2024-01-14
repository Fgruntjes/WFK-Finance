using App.Backend.Dto;
using App.Institution.Exception;
using App.Institution.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(InstitutionConnectionListController.RouteBase)]
public class InstitutionConnectionRefreshController : ControllerBase
{
    public const string RouteNameByExternalId = $"{nameof(InstitutionConnectionListController)}_ByExternalId";
    public const string RouteNameById = $"{nameof(InstitutionConnectionListController)}_ById";

    private readonly IInstitutionConnectionRefreshService _refreshService;

    public InstitutionConnectionRefreshController(IInstitutionConnectionRefreshService refreshService)
    {
        _refreshService = refreshService;
    }

    [HttpPatch("refresh/external/{externalId}", Name = RouteNameByExternalId)]
    [ProducesResponseType(typeof(InstitutionConnectionDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Refresh(string externalId, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _refreshService.Refresh(externalId, cancellationToken);
            return AcceptedAtRoute(
                InstitutionConnectionGetController.RouteName,
                new { id = entity.Id },
                entity.ToDto());
        }
        catch (InstitutionConnectionNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("refresh/id/{id:guid}", Name = RouteNameById)]
    [ProducesResponseType(typeof(InstitutionConnectionDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Refresh(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _refreshService.Refresh(id, cancellationToken);
            return AcceptedAtRoute(
                InstitutionConnectionGetController.RouteName,
                new { id = entity.Id },
                entity.ToDto());
        }
        catch (InstitutionConnectionNotFoundException)
        {
            return NotFound();
        }
    }
}
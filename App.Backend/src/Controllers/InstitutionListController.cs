using App.Backend.Dto;
using App.Lib.InstitutionConnection.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(InstitutionGetController.RouteBase)]
public class InstitutionListController : ControllerBase
{
    public const string RouteName = nameof(InstitutionListController);

    private readonly IInstitutionSearchService _searchService;

    public InstitutionListController(IInstitutionSearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("{countryIso2:length(2)}", Name = RouteName)]
    [ProducesResponseType(typeof(ICollection<Institution>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List(
        string countryIso2,
        CancellationToken cancellationToken = default)
    {
        var result = (await _searchService.Search(countryIso2, cancellationToken)).ToList();
        if (!result.Any())
        {
            return NotFound();
        }

        return Ok(result.Select(e => e.ToDto()));
    }
}
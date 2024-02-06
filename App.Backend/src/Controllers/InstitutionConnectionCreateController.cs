using App.Backend.Dto;
using App.Institution.Interface;
using App.Lib.Data.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(InstitutionConnectionListController.RouteBase)]
public class InstitutionConnectionCreateController : ControllerBase
{
    public const string RouteName = nameof(InstitutionConnectionCreateController);

    private readonly IInstitutionConnectionCreateService _createService;

    public InstitutionConnectionCreateController(IInstitutionConnectionCreateService createService)
    {
        _createService = createService;
    }

    [ProducesResponseType(typeof(InstitutionConnectionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpPost(Name = RouteName)]
    public async Task<IActionResult> Create(
        [FromBody] InstitutionConnectionInputDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _createService.Connect(request.InstitutionId, request.ReturnUrl, cancellationToken);
            return CreatedAtRoute(
                InstitutionConnectionGetController.RouteName,
                new { id = entity.Id },
                entity.ToDto());
        }
        catch (EntityNotFoundException exception)
        {
            return BadRequest(exception.ToProblemDetails());
        }
    }
}
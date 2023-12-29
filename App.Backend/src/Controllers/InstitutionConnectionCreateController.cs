using App.Backend.Dto;
using App.Lib.InstitutionConnection.Exception;
using App.Lib.InstitutionConnection.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(InstitutionConnectionGetController.RouteBase)]
public class InstitutionConnectionCreateController : ControllerBase
{
    public const string RouteName = nameof(InstitutionConnectionCreateController);

    private readonly IInstitutionConnectionCreateService _createService;

    public InstitutionConnectionCreateController(IInstitutionConnectionCreateService createService)
    {
        _createService = createService;
    }

    [ProducesResponseType(typeof(InstitutionConnection), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpPost(Name = RouteName)]
    public async Task<IActionResult> Create(
        [FromBody] InstitutionConnectionCreate request,
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
        catch (InstitutionNotFoundException exception)
        {
            return BadRequest(exception.ToProblemDetails());
        }
    }
}
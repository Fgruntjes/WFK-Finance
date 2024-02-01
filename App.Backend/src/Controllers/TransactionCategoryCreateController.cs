using App.Backend.Dto;
using App.TransactionCategory.Exception;
using App.TransactionCategory.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(TransactionCategoryListController.RouteBase)]
public class TransactionCategoryCreateController : ControllerBase
{
    public const string RouteName = nameof(TransactionCategoryCreateController);
    private readonly ITransactionCategoryService _createService;

    public TransactionCategoryCreateController(ITransactionCategoryService createService)
    {
        _createService = createService;
    }

    [ProducesResponseType(typeof(TransactionCategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpPost(Name = RouteName)]
    public async Task<IActionResult> Create(
        [FromBody] TransactionCategoryInputDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _createService.CreateAsync(
                request.Name,
                request.Group,
                request.ParentId,
                request.SortOrder,
                request.Description,
                cancellationToken);

            return CreatedAtRoute(
                TransactionCategoryGetController.RouteName,
                new { id = entity.Id },
                entity.ToDto());
        }
        catch (CategoryNotFoundException exception)
        {
            return BadRequest(exception.ToProblemDetails());
        }
    }
}

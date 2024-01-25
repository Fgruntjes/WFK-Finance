using App.Backend.Dto;
using App.TransactionCategory.Exception;
using App.TransactionCategory.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(TransactionCategoryListController.RouteBase)]
public class TransactionCategoryUpdateController : ControllerBase
{
    public const string RouteName = nameof(TransactionCategoryUpdateController);
    private readonly ITransactionCategoryService _createService;

    public TransactionCategoryUpdateController(ITransactionCategoryService createService)
    {
        _createService = createService;
    }

    [ProducesResponseType(typeof(TransactionCategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpPut("{id:guid}", Name = RouteName)]
    public async Task<IActionResult> Create(
        Guid id,
        [FromBody] TransactionCategoryInputDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _createService.UpdateAsync(
                id,
                request.Name,
                request.Group,
                request.ParentId,
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
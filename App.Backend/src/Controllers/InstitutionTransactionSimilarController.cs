using App.Backend.Dto;
using App.Lib.Data.Exception;
using App.TransactionCategory.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(InstitutionTransactionListController.RouteBase)]
public class InstitutionTransactionSimilarController : ControllerBase
{
    public const string RouteName = nameof(InstitutionTransactionSimilarController);

    private readonly ISimilarTransactionService _transactionService;

    public InstitutionTransactionSimilarController(ISimilarTransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("{id:guid}/similar", Name = RouteName)]
    [ProducesResponseType(typeof(ICollection<InstitutionTransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Similar(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var transactions = await _transactionService.Find(id, cancellationToken);
            return Ok(transactions.Select(e => e.ToDto()));
        }
        catch (EntityNotFoundException exception)
        {
            return NotFound(exception.ToProblemDetails(System.Net.HttpStatusCode.NotFound));
        }
    }
}

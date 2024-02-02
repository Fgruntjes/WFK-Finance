using App.Backend.Dto;
using App.Lib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(InstitutionTransactionListController.RouteBase)]
public class InstitutionTransactionPatchController : ControllerBase
{
    public const string RouteName = nameof(InstitutionTransactionPatchController);

    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;

    public InstitutionTransactionPatchController(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [HttpPatch(Name = RouteName)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Patch(
        [FromQuery] ICollection<Guid> id,
        [FromBody] InstitutionTransactionPatchDto request,
        CancellationToken cancellationToken = default)
    {
        // Get the list of InstitutionTransactions for the given AccountIds
        var transactions = _database.InstitutionAccountTransactions
            .Where(e => e.Account.InstitutionConnection.OrganisationId == _organisationIdProvider.GetOrganisationId())
            .Where(t => id.Contains(t.Id));

        if (request.CategoryId != null)
        {
            // Ensure the CategoryId is valid
            var category = await _database.TransactionCategory
                .Where(c => c.OrganisationId == _organisationIdProvider.GetOrganisationId())
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

            if (category == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Category not found",
                    Detail = "The category does not exist or is not accessible",
                    Status = StatusCodes.Status400BadRequest,
                });
            }
        }

        if (transactions.Count() != id.Count)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Transaction not found",
                Detail = "The transaction does not exist or is not accessible",
                Status = StatusCodes.Status400BadRequest,
            });
        }

        // Update the CategoryId for each transaction
        foreach (var transaction in transactions)
        {
            transaction.CategoryId = request.CategoryId;
        }

        // Save the changes to the database
        await _database.SaveChangesAsync();

        return NoContent();
    }
}


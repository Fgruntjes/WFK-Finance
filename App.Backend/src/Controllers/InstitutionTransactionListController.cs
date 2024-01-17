using App.Backend.Dto;
using App.Backend.Mvc;
using App.Lib.Data;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[ApiController]
[ApiGroup(typeof(InstitutionAccountListController))]
[Authorize]
[Route(RouteBase)]
public class InstitutionTransactionListController : ControllerBase
{
    public const string RouteBase = "/institutionaccounts/{id:guid}/transactions";

    public const string RouteName = nameof(InstitutionTransactionListController);

    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;

    public InstitutionTransactionListController(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [HttpGet(Name = RouteName)]
    [ProducesResponseType(typeof(ICollection<InstitutionAccountTransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List(
        [FromRoute] Guid id,
        [FromQuery] GridifyQuery query,
        CancellationToken cancellationToken = default)
    {
        var accountEntity = await _database.InstitutionAccounts
            .Where(e => e.Id == id)
            .Where(e => e.InstitutionConnection.OrganisationId == _organisationIdProvider.GetOrganisationId())
            .SingleOrDefaultAsync(cancellationToken);
        if (accountEntity == null)
            return NotFound();

        var result = await _database.InstitutionAccountTransactions
            .Where(e => e.AccountId == id)
            .GridifyAsync(query, cancellationToken);

        return ListResult<InstitutionAccountTransactionDto>.Create(
            "institutionaccounttransactions",
            query,
            result,
            entity => entity.ToDto());
    }
}

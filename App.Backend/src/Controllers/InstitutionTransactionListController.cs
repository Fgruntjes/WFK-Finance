using App.Backend.Dto;
using App.Backend.Gridify;
using App.Backend.Mvc;
using App.Lib.Data;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(RouteBase)]
public class InstitutionTransactionListController : ControllerBase
{
    public const string RouteBase = "/institutiontransactions";

    public const string RouteName = nameof(InstitutionTransactionListController);

    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;

    public InstitutionTransactionListController(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [HttpGet(Name = RouteName)]
    [ProducesResponseType(typeof(ICollection<InstitutionTransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] GridifyQuery query,
        CancellationToken cancellationToken = default)
    {
        var accountQuery = _database.InstitutionAccounts
            .Where(e => e.InstitutionConnection.OrganisationId == _organisationIdProvider.GetOrganisationId())
            .Select(a => a.Id);

        var result = await _database.InstitutionAccountTransactions
            .Where(e => accountQuery.Contains(e.AccountId))
            .GridifyQueryableAsync(query, new InstitutionTransactionQueryMapper(), cancellationToken);

        var items = await result.Query
            .Include(e => e.Account)
            .Include(e => e.Account.InstitutionConnection)
            .Select(e => new InstitutionTransactionDto
            {
                Id = e.Id,
                InstitutionId = e.Account.InstitutionConnection.InstitutionId,
                AccountIban = e.Account.Iban,
                Amount = e.Amount,
                CounterPartyAccount = e.CounterPartyAccount,
                CounterPartyName = e.CounterPartyName,
                Currency = e.Currency,
                Date = e.Date.ToDateTimeUtc(),
                UnstructuredInformation = e.UnstructuredInformation,
            })
            .ToListAsync(cancellationToken);

        var start = query.Page * query.PageSize;
        return ListResult<InstitutionTransactionDto>.Create(items, RouteBase, query, result.Count);
    }
}
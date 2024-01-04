using App.Backend.Dto;
using App.Backend.Exception;
using App.Backend.Linq;
using App.Backend.Mvc;
using App.Lib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[ApiController]
[ApiGroup(typeof(InstitutionAccountListController))]
[Authorize]
[Route(RouteBase)]
public class InstitutionAccountTransactionListController : ControllerBase
{
    public const string RouteBase = "/institutionaccount/{id:guid}/transactions";

    public const string RouteName = nameof(InstitutionAccountTransactionListController);

    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;

    public InstitutionAccountTransactionListController(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [HttpGet(Name = RouteName)]
    [ProducesResponseType(typeof(ICollection<InstitutionAccountTransaction>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List(
        [FromRoute] Guid id,
        [FromQuery] RangeParameter? range,
        [FromQuery] FilterParameter? filter,
        [FromQuery] SortParameter? sort,
        CancellationToken cancellationToken = default)
    {
        sort ??= new SortParameter();
        range ??= new RangeParameter();
        filter ??= new FilterParameter();

        var accountEntity = await _database.InstitutionAccounts
            .Where(e => e.Id == id)
            .Where(e => e.InstitutionConnection.OrganisationId == _organisationIdProvider.GetOrganisationId())
            .SingleOrDefaultAsync(cancellationToken);
        if (accountEntity == null)
            return NotFound();

        var organisationId = _organisationIdProvider.GetOrganisationId();
        try
        {
            var query = _database.InstitutionAccountTransactions
                .Where(e => e.AccountId == id)
                .ApplyFilter(filter);

            var totalCount = await query.CountAsync(cancellationToken);
            var result = await query
                .OrderBy(e => e.CreatedAt)
                .ApplySort(sort)
                .ApplyRange(range)
                .Select(e => e.ToDto())
                .ToListAsync(cancellationToken);

            return new ListResult<InstitutionAccountTransaction>(
                result,
                "institutionaccounttransactions",
                range,
                totalCount);
        }
        catch (InvalidPropertyException e)
        {
            var problemDetails = e.ToProblemDetails();
            problemDetails.Extensions["Type"] = typeof(InstitutionAccountTransaction).Name;
            return BadRequest(problemDetails);
        }
    }
}

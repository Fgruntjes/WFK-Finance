using App.Backend.Dto;
using App.Lib.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(InstitutionAccountListController.RouteBase)]
public class InstitutionAccountGetController : ControllerBase
{
    public const string RouteName = nameof(InstitutionAccountGetController);

    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;

    public InstitutionAccountGetController(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [HttpGet("{id:guid}", Name = RouteName)]
    [ProducesResponseType(typeof(InstitutionAccount), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.GetOrganisationId();
        var account = await _database.InstitutionAccounts
            .Where(e => e.InstitutionConnection.OrganisationId == organisationId)
            .Where(e => e.Id == id)
            .Select(e => new InstitutionAccount
            {
                Id = e.Id,
                ExternalId = e.ExternalId,
                Iban = e.Iban,
                ImportStatus = e.ImportStatus,
                LastImport = e.LastImport.HasValue ? e.LastImport.Value.ToDateTimeUtc() : null,
                TransactionCount = e.Transactions.Count
            })
            .Take(1)
            .SingleOrDefaultAsync(cancellationToken);

        if (account == null)
        {
            return NotFound();
        }

        return Ok(account);
    }
}

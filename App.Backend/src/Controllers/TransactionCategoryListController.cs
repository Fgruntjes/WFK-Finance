using App.Backend.Dto;
using App.Backend.Mvc;
using App.Lib.Data;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(RouteBase)]
public class TransactionCategoryListController : ControllerBase
{
    public const string RouteBase = "/transactioncategories";

    public const string RouteName = nameof(TransactionCategoryListController);

    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;

    public TransactionCategoryListController(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [HttpGet(Name = RouteName)]
    [ProducesResponseType(typeof(ICollection<TransactionCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] GridifyQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _database.TransactionCategory
            .Where(e => e.OrganisationId == _organisationIdProvider.GetOrganisationId())
            .GridifyAsync(query, cancellationToken);

        return ListResult<TransactionCategoryDto>.Create(
            RouteBase,
            query,
            result.Data.Select(e => e.ToDto()),
            result.Count);
    }
}
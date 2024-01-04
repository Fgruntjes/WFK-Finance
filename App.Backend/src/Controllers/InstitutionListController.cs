using System.Text.Json;
using App.Backend.Dto;
using App.Backend.Linq;
using App.Backend.Mvc;
using App.Backend.OpenApi;
using App.Lib.Data;
using App.Lib.Data.Entity;
using App.Lib.InstitutionConnection.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(RouteBase)]
public class InstitutionListController : ControllerBase
{
    public const string RouteBase = "/institutions";
    public const string RouteName = nameof(InstitutionListController);

    private readonly IInstitutionSearchService _searchService;
    private readonly DatabaseContext _database;

    public InstitutionListController(IInstitutionSearchService searchService, DatabaseContext database)
    {
        _searchService = searchService;
        _database = database;
    }

    [HttpGet(Name = RouteName)]
    [ProducesResponseType(typeof(ICollection<Institution>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List(
        [FromQuery] InstitutionFilterParameter filter,
        [FromQuery] RangeParameter? range,
        CancellationToken cancellationToken = default)
    {
        range ??= new RangeParameter(0, 1000);
        if (filter.Id == null && filter.CountryIso2 == null)
            return BadRequest();

        var (result, count) = await CreateResultList(filter, range, cancellationToken);

        return new ListResult<Institution>(
            result.Select(e => e.ToDto()),
            RouteBase,
            range,
            count);
    }

    private async Task<(IList<InstitutionEntity>, int)> CreateResultList(
        InstitutionFilterParameter filter,
        RangeParameter? range,
        CancellationToken cancellationToken)
    {
        if (filter.Id != null)
        {
            var query = _database.Institutions.Where(e => filter.Id.Contains(e.Id));
            if (filter.CountryIso2 != null)
            {
                query = query.Where(e => e.CountryIso2 == filter.CountryIso2);
            }

            var count = await query.CountAsync(cancellationToken);
            if (range != null)
            {
                query = query.ApplyRange(range);
            }

            var list = await query.ToListAsync(cancellationToken);
            return (list, count);
        }

        if (filter.CountryIso2 != null)
        {
            var list = (await _searchService.Search(filter.CountryIso2, cancellationToken)).ToList();
            return (list, list.Count);
        }

        throw new System.Exception("Invalid filter");
    }
}
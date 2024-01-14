using System.Text.RegularExpressions;
using App.Backend.Dto;
using App.Backend.Mvc;
using App.Lib.Data;
using App.Institution.Service;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(RouteBase)]
public partial class InstitutionListController : ControllerBase
{
    [GeneratedRegex("countryiso2 *= *([a-z]{2})", RegexOptions.IgnoreCase)]
    private static partial Regex CountryFilterRegex();

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
    [ProducesResponseType(typeof(ICollection<Dto.Institution>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List(
        [FromQuery] GridifyQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _database.Institutions.GridifyAsync(query, cancellationToken);
        if (result.Count == 0)
        {
            var countryIso2 = FindCountryFilter(query.Filter);
            if (countryIso2 != null)
            {
                result = (await _searchService.Search(countryIso2, cancellationToken))
                    .AsQueryable()
                    .Gridify(query);
            }
        }

        return ListResult<Dto.Institution>.Create(RouteBase, query, result, entity => entity.ToDto());
    }

    private static string? FindCountryFilter(string? filter)
    {
        if (filter == null)
        {
            return null;
        }

        var matches = CountryFilterRegex().Matches(filter);
        if (matches.Count == 0)
        {
            return null;
        }

        return matches[0].Groups[1].Value;
    }
}
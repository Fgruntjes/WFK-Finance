using App.Backend.GraphQL.Type;
using App.Lib.InstitutionConnection.Service;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace App.Backend.Controllers;

[GraphRoute("institution")]
public class InstitutionListController : GraphController
{
    private readonly IInstitutionSearchService _searchService;

    public InstitutionListController(IInstitutionSearchService searchService)
    {
        _searchService = searchService;
    }

    [Authorize]
    [Query("list", TypeExpression = $"[Type!]!")]
    public async Task<IEnumerable<Institution>> List(
        string countryIso2,
        CancellationToken cancellationToken = default)
    {
        return (await _searchService.Search(countryIso2, cancellationToken))
            .Select(e => e.ToGraphQLType());
    }
}
using App.Backend.GraphQL.Type;
using App.Backend.Service;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace App.Backend.Controllers;

[GraphRoute("institution")]
public class InstitutionListController : GraphController
{
	private readonly InstitutionSearchService _searchService;

	public InstitutionListController(InstitutionSearchService searchService)
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
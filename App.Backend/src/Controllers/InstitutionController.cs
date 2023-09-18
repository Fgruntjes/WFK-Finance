using App.Backend.Data;
using App.Backend.GraphQL.Type;
using App.Backend.Service;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[GraphRoute("institution")]
public class InstitutionController : GraphController
{
	private readonly DatabaseContext _database;
	private readonly InstitutionSearchService _searchService;

	public InstitutionController(DatabaseContext database, InstitutionSearchService searchService)
	{
		_database = database;
		_searchService = searchService;
	}

	[Authorize]
	[Query("get")]
	public Task<Institution?> Get(Guid id, CancellationToken cancellationToken = default)
	{
		return _database.Institutions
			.Where(e => e.Id == id)
			.OrderBy(e => e.CreatedAt)
			.Take(1)
			.Select(e => Institution.FromEntity(e))
			.SingleOrDefaultAsync(cancellationToken);
	}

	[Authorize]
	[Query("list", TypeExpression = $"[Type!]!")]
	public async Task<IEnumerable<Institution>> List(
		string countryIso2,
		CancellationToken cancellationToken = default)
	{
		return (await _searchService.Search(countryIso2, cancellationToken))
			.Select(e => Institution.FromEntity(e));
	}
}
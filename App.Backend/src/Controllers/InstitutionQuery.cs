using System.Collections.Immutable;
using App.Backend.Data;
using App.Backend.Dto;
using App.Backend.GraphQL.Type;
using App.Backend.Service;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

public class InstitutionQuery : GraphController
{
	private readonly DatabaseContext _database;
	private readonly InstitutionSearchService _searchService;

	public InstitutionQuery(DatabaseContext database, InstitutionSearchService searchService)
	{
		_database = database;
		_searchService = searchService;
	}

	[Authorize]
	[QueryRoot("institution")]
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
	[QueryRoot("institutions")]
	public async Task<ListResult<Institution>> List(
		string countryIso3,
		int offset = 0,
		int limit = 25,
		CancellationToken cancellationToken = default)
	{
		var result = await _searchService.Search(countryIso3, offset, limit, cancellationToken);
		return new ListResult<Institution>
		{
			Items = result.Items
				.Select(e => Institution.FromEntity(e))
				.ToImmutableList(),
			TotalCount = result.TotalCount,
		};
	}
}
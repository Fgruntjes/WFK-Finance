using App.Data;
using App.Backend.GraphQL.Type;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[GraphRoute("institution")]
public class InstitutionGetController : GraphController
{
	private readonly DatabaseContext _database;

	public InstitutionGetController(DatabaseContext database)
	{
		_database = database;
	}

	[Authorize]
	[Query("get")]
	public Task<Institution?> Get(Guid id, CancellationToken cancellationToken = default)
	{
		return _database.Institutions
			.Where(e => e.Id == id)
			.OrderBy(e => e.CreatedAt)
			.Take(1)
			.Select(e => e.ToGraphQLType())
			.SingleOrDefaultAsync(cancellationToken);
	}
}
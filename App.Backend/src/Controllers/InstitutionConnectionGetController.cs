using App.Data;
using App.Backend.GraphQL.Type;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[GraphRoute("institutionConnection")]
public class InstitutionConnectionGetController : GraphController
{
	private readonly DatabaseContext _database;
	private readonly AppHttpContext _httpContext;

	public InstitutionConnectionGetController(DatabaseContext database, AppHttpContext httpContext)
	{
		_database = database;
		_httpContext = httpContext;
	}

	[Authorize]
	[Query("get")]
	public async Task<InstitutionConnection?> Get(Guid id, CancellationToken cancellationToken = default)
	{
		var organisationId = await _httpContext.OrganisationIdAsync(cancellationToken);
		return await _database.InstitutionConnections
			.Where(e => e.Id == id && e.OrganisationId == organisationId)
			.OrderBy(e => e.CreatedAt)
			.Take(1)
			.Select(e => e.ToGraphQLType())
			.SingleOrDefaultAsync(cancellationToken);
	}
}
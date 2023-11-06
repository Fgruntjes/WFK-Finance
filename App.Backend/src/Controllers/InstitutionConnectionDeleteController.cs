using App.Data;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[GraphRoute("institutionConnection")]
public class InstitutionConnectionDeleteController : GraphController
{
	private readonly DatabaseContext _database;
	private readonly AppHttpContext _httpContext;

	public InstitutionConnectionDeleteController(DatabaseContext database, AppHttpContext httpContext)
	{
		_database = database;
		_httpContext = httpContext;
	}

	[Authorize]
	[Mutation("delete")]
	public async Task<int> Delete(ICollection<Guid> connectionIds, CancellationToken cancellationToken = default)
	{
		var organisationId = await _httpContext.OrganisationIdAsync(cancellationToken);
		var entities = _database.InstitutionConnections
				.Where(e => e.OrganisationId == organisationId)
				.Where(e => connectionIds.Contains(e.Id));
		var deleteCount = await entities.CountAsync(cancellationToken);

		_database.InstitutionConnections.RemoveRange(entities);
		await _database.SaveChangesAsync(cancellationToken);

		return deleteCount;
	}
}
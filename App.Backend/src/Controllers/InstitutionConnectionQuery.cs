using System.Collections.Immutable;
using App.Backend.Data;
using App.Backend.GraphQL.Type;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

public class InstitutionConnectionQuery : GraphController
{
	private readonly DatabaseContext _database;

	public InstitutionConnectionQuery(DatabaseContext database)
	{
		_database = database;
	}

	[Authorize]
	[QueryRoot("institutionConnection")]
	public Task<InstitutionConnection?> Get(Guid id)
	{
		return _database.InstitutionConnections
			.Where(e => e.Id == id)
			.Take(1)
			.Select(e => InstitutionConnection.FromEntity(e))
			.SingleOrDefaultAsync();
	}

	[Authorize]
	[QueryRoot("institutionConnections")]
	public async Task<ListResult<InstitutionConnection>> List(int offset = 0, int limit = 25)
	{
		var query = _database.InstitutionConnections;

		var totalCount = await query.CountAsync();
		var result = await query
			.Skip(offset)
			.Take(limit)
			.OrderBy(e => e.Id)
			.Select(e => InstitutionConnection.FromEntity(e))
			.ToListAsync();

		return new ListResult<InstitutionConnection>
		{
			Items = result,
			TotalCount = totalCount
		};
	}

	[Authorize]
	[TypeExtension(typeof(InstitutionConnection), "organisation")]
	public async Task<Organisation> GetOrganisation(InstitutionConnection connection)
	{
		return await _database.Organisations
			.Where(e => e.Id == connection.OrganisationId)
			.Select(e => Organisation.FromEntity(e))
			.SingleAsync();
	}

	[Authorize]
	[TypeExtension(typeof(InstitutionConnection), "institution")]
	public async Task<Institution> GetInstitution(InstitutionConnection connection)
	{
		return await _database.Institutions
			.Where(e => e.Id == connection.InstitutionId)
			.Select(e => Institution.FromEntity(e))
			.SingleAsync();
	}

	[Authorize]
	[BatchTypeExtension(typeof(InstitutionConnection), "accounts", typeof(List<InstitutionConnectionAccount>))]
	public IGraphActionResult GetAccounts(IEnumerable<InstitutionConnection> connections, int limit = 25)
	{
		var connectionIds = connections.Select(c => c.Id).ToArray();

		var allAccounts = _database
			.InstitutionConnectionAccounts
			.Where(e => connectionIds.Contains(e.InstitutionConnectionId))
			.Take(limit);

		return StartBatch()
			.FromSource(connections, c => c.Id)
			.WithResults(allAccounts, a => a.InstitutionConnectionId)
			.Complete();
	}
}


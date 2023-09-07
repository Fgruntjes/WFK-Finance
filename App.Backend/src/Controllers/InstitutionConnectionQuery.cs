using System.Collections.Immutable;
using App.Backend.Data;
using App.Backend.GraphQL.Type;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

public class InstitutionConnectionMutation : GraphController
{
}

public class InstitutionConnectionQuery : GraphController
{
	private readonly DatabaseContext _database;

	public InstitutionConnectionQuery(DatabaseContext database)
	{
		_database = database;
	}

	[Authorize]
	[QueryRoot("institutionConnection")]
	public Task<InstitutionConnection?> Get(Guid id, CancellationToken cancellationToken = default)
	{
		return _database.InstitutionConnections
			.Where(e => e.Id == id)
			.Take(1)
			.Select(e => InstitutionConnection.FromEntity(e))
			.SingleOrDefaultAsync(cancellationToken);
	}

	[Authorize]
	[QueryRoot("institutionConnections")]
	public async Task<ListResult<InstitutionConnection>> List(int offset = 0, int limit = 25, CancellationToken cancellationToken = default)
	{
		var query = _database.InstitutionConnections;

		var totalCount = await query.CountAsync();
		var result = await query
			.Skip(offset)
			.Take(limit)
			.OrderBy(e => e.Id)
			.Select(e => InstitutionConnection.FromEntity(e))
			.ToListAsync(cancellationToken);

		return new ListResult<InstitutionConnection>
		{
			Items = result,
			TotalCount = totalCount
		};
	}

	[Authorize]
	[TypeExtension(typeof(InstitutionConnection), "organisation")]
	public async Task<Organisation> GetOrganisation(InstitutionConnection connection, CancellationToken cancellationToken = default)
	{
		return await _database.Organisations
			.Where(e => e.Id == connection.OrganisationId)
			.Select(e => Organisation.FromEntity(e))
			.SingleAsync(cancellationToken);
	}

	[Authorize]
	[TypeExtension(typeof(InstitutionConnection), "institution")]
	public async Task<Institution> GetInstitution(InstitutionConnection connection, CancellationToken cancellationToken = default)
	{
		return await _database.Institutions
			.Where(e => e.Id == connection.InstitutionId)
			.Select(e => Institution.FromEntity(e))
			.SingleAsync(cancellationToken);
	}

	[Authorize]
	[BatchTypeExtension(typeof(InstitutionConnection), "accounts", typeof(List<InstitutionConnectionAccount>))]
	public async Task<IGraphActionResult> GetAccounts(IEnumerable<InstitutionConnection> connections, int limit = 25, CancellationToken cancellationToken = default)
	{
		var connectionIds = connections.Select(c => c.Id).ToArray();

		var allAccounts = await _database
			.InstitutionConnectionAccounts
			.Where(e => connectionIds.Contains(e.InstitutionConnectionId))
			.Take(limit)
			.ToListAsync(cancellationToken);

		return StartBatch()
			.FromSource(connections, c => c.Id)
			.WithResults(allAccounts, a => a.InstitutionConnectionId)
			.Complete();
	}
}


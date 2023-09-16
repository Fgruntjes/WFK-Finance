using System.Collections.Immutable;
using App.Backend.Data;
using App.Backend.Dto;
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
	private readonly IHttpContextAccessor _httpContextAccessor;

	public InstitutionConnectionQuery(DatabaseContext database, IHttpContextAccessor httpContextAccessor)
	{
		_database = database;
		_httpContextAccessor = httpContextAccessor;
	}

	[Authorize]
	[QueryRoot("institutionConnection")]
	public Task<InstitutionConnection?> Get(Guid id, CancellationToken cancellationToken = default)
	{
		return _database.InstitutionConnections
			.Where(e => e.Id == id && e.OrganisationId == _httpContextAccessor.GetOrganisationId())
			.OrderBy(e => e.CreatedAt)
			.Take(1)
			.Select(e => InstitutionConnection.FromEntity(e))
			.SingleOrDefaultAsync(cancellationToken);
	}

	[Authorize]
	[QueryRoot("institutionConnections")]
	public async Task<ListResult<InstitutionConnection>> List(int offset = 0, int limit = 25, CancellationToken cancellationToken = default)
	{
		var query = _database.InstitutionConnections
			.Where(e => e.OrganisationId == _httpContextAccessor.GetOrganisationId());

		var totalCount = await query.CountAsync();
		var result = await query
			.OrderBy(e => e.CreatedAt)
			.Skip(offset)
			.Take(limit)
			.Select(e => InstitutionConnection.FromEntity(e))
			.ToListAsync(cancellationToken);

		return new ListResult<InstitutionConnection>
		{
			Items = result,
			TotalCount = totalCount
		};
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
			.OrderBy(e => e.CreatedAt)
			.Take(limit)
			.ToListAsync(cancellationToken);

		return StartBatch()
			.FromSource(connections, c => c.Id)
			.WithResults(allAccounts, a => a.InstitutionConnectionId)
			.Complete();
	}
}


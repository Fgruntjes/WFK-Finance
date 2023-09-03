using App.Backend.Data;
using App.Backend.GraphQL.Type;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

public class InstitutionConnectionController : GraphController
{
	private readonly DatabaseContext _database;

	public InstitutionConnectionController(DatabaseContext database)
	{
		_database = database;
	}
	
	[Authorize]
	[QueryRoot("institutionConnection")]
	public Task<InstitutionConnection?> Get(Guid id)
	{
		return _database.InstitutionConnections
			.Where(e => e.Id == id)
			.Select(e => InstitutionConnection.FromEntity(e))
			.SingleOrDefaultAsync();
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
	public async Task<IGraphActionResult> GetAccounts(IEnumerable<InstitutionConnection> connections, int limit = 25)
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
using App.Backend.Data;
using App.Backend.GraphQL.Type;
using App.Backend.Service;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[GraphRoute("institutionConnection")]
public class InstitutionConnectionController : GraphController
{
	private readonly DatabaseContext _database;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly InstitutionConnectionCreateService _createService;
	private readonly InstitutionConnectionRefreshService _refreshService;

	public InstitutionConnectionController(
		DatabaseContext database,
		IHttpContextAccessor httpContextAccessor,
		InstitutionConnectionCreateService createService,
		InstitutionConnectionRefreshService refreshService)
	{
		_database = database;
		_httpContextAccessor = httpContextAccessor;
		_createService = createService;
		_refreshService = refreshService;
	}

	[Authorize]
	[Query("get")]
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
	[Query("list")]
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

	[TypeExtension(typeof(InstitutionConnection), "institution")]
	public async Task<Institution> GetInstitution(InstitutionConnection connection, CancellationToken cancellationToken = default)
	{
		return await _database.Institutions
			.Where(e => e.Id == connection.InstitutionId)
			.Select(e => Institution.FromEntity(e))
			.SingleAsync(cancellationToken);
	}

	[TypeExtension(typeof(InstitutionConnection), "accounts", TypeExpression = "[Type!]!")]
	public async Task<IList<InstitutionConnectionAccount>> GetAccounts(InstitutionConnection connection, CancellationToken cancellationToken = default)
	{
		return await _database
			.InstitutionConnectionAccounts
			.Where(e => e.InstitutionConnectionId == connection.Id)
			.Select(e => InstitutionConnectionAccount.FromEntity(e))
			.ToListAsync(cancellationToken);
	}

	[Authorize]
	[Mutation("create", typeof(InstitutionConnection))]
	public async Task<IGraphActionResult> Create(Guid institutionId, [FromGraphQL(TypeExpression = "Type!")] Uri returnUrl, CancellationToken cancellationToken = default)
	{
		try
		{
			var entity = await _createService.Connect(institutionId, returnUrl, cancellationToken);
			return Ok(InstitutionConnection.FromEntity(entity));
		}
		catch (ArgumentOutOfRangeException exception)
		{
			return BadRequest(exception.Message);
		}
	}

	[Authorize]
	[Mutation("refresh/externalId", typeof(InstitutionConnection))]
	public async Task<IGraphActionResult> Refresh(string externalId, CancellationToken cancellationToken = default)
	{
		try
		{
			var entity = await _refreshService.Refresh(externalId, cancellationToken);
			return Ok(InstitutionConnection.FromEntity(entity));
		}
		catch (InvalidOperationException)
		{
			return BadRequest("Connection not found");
		}
	}

	[Authorize]
	[Mutation("refresh/id", typeof(InstitutionConnection))]
	public async Task<IGraphActionResult> Refresh(Guid id, CancellationToken cancellationToken = default)
	{
		try
		{
			var entity = await _refreshService.Refresh(id, cancellationToken);
			return Ok(InstitutionConnection.FromEntity(entity));
		}
		catch (InvalidOperationException)
		{
			return BadRequest("Connection not found");
		}
	}

	[Authorize]
	[Mutation("delete")]
	public async Task<int> Delete(ICollection<Guid> connectionIds, CancellationToken cancellationToken = default)
	{
		var organisationId = _httpContextAccessor.GetOrganisationId();
		var entities = _database.InstitutionConnections
				.Where(e => e.OrganisationId == organisationId)
				.Where(e => connectionIds.Contains(e.Id));
		var deleteCount = await entities.CountAsync(cancellationToken);

		_database.InstitutionConnections.RemoveRange(entities);
		await _database.SaveChangesAsync(cancellationToken);

		return connectionIds.Count();
	}
}
using App.Data;
using App.Backend.GraphQL.Type;
using App.Backend.Service;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[GraphRoute("institutionConnection")]
public class InstitutionConnectionListController : GraphController
{
    private readonly DatabaseContext _database;
    private readonly OrganisationIdProvider _organisationIdProvider;

    public InstitutionConnectionListController(DatabaseContext database, OrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [Authorize]
    [Query("list")]
    public async Task<ListResult<InstitutionConnection>> List(int offset = 0, int limit = 25, CancellationToken cancellationToken = default)
    {
        var organisationId = await _organisationIdProvider.OrganisationIdAsync(cancellationToken);
        var query = _database.InstitutionConnections
            .Where(e => e.OrganisationId == organisationId);

        var totalCount = await query.CountAsync();
        var result = await query
            .OrderBy(e => e.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .Select(e => e.ToGraphQLType())
            .ToListAsync(cancellationToken);

        return new ListResult<InstitutionConnection>
        {
            Items = result,
            TotalCount = totalCount
        };
    }
}
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
    private readonly OrganisationIdProvider _organisationIdProvider;

    public InstitutionConnectionGetController(DatabaseContext database, OrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [Authorize]
    [Query("get")]
    public async Task<InstitutionConnection?> Get(Guid id, CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.GetOrganisationId(User);
        return await _database.InstitutionConnections
            .Where(e => e.Id == id && e.OrganisationId == organisationId)
            .OrderBy(e => e.CreatedAt)
            .Take(1)
            .Select(e => e.ToGraphQLType())
            .SingleOrDefaultAsync(cancellationToken);
    }
}
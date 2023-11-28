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
    private readonly OrganisationIdProvider _organisationIdProvider;

    public InstitutionConnectionDeleteController(DatabaseContext database, OrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    [Authorize]
    [Mutation("delete")]
    public async Task<int> Delete(ICollection<Guid> connectionIds, CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.GetOrganisationId(User);
        var entities = _database.InstitutionConnections
                .Where(e => e.OrganisationId == organisationId)
                .Where(e => connectionIds.Contains(e.Id));
        var deleteCount = await entities.CountAsync(cancellationToken);

        _database.InstitutionConnections.RemoveRange(entities);
        await _database.SaveChangesAsync(cancellationToken);

        return deleteCount;
    }
}
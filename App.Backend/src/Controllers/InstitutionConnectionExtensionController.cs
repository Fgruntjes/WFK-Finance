using App.Lib.Data;
using App.Backend.GraphQL.Type;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

[GraphRoute("institutionConnection")]
public class InstitutionConnectionExtensionController : GraphController
{
    private readonly DatabaseContext _database;

    public InstitutionConnectionExtensionController(DatabaseContext database)
    {
        _database = database;
    }

    [TypeExtension(typeof(InstitutionConnection), "institution")]
    public async Task<Institution> GetInstitution(InstitutionConnection connection, CancellationToken cancellationToken = default)
    {
        return await _database.Institutions
            .Where(e => e.Id == connection.InstitutionId)
            .Select(e => e.ToGraphQLType())
            .SingleAsync(cancellationToken);
    }

    [TypeExtension(typeof(InstitutionConnection), "accounts", TypeExpression = "[Type!]!")]
    public async Task<IList<InstitutionConnectionAccount>> GetAccounts(InstitutionConnection connection, CancellationToken cancellationToken = default)
    {
        return await _database
            .InstitutionConnectionAccounts
            .Where(e => e.InstitutionConnectionId == connection.Id)
            .OrderBy(e => e.CreatedAt)
            .Select(e => e.ToGraphQLType())
            .ToListAsync(cancellationToken);
    }
}
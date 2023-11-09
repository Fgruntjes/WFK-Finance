using App.Data;
using Microsoft.EntityFrameworkCore;

namespace App;

public class OrganisationIdProvider
{
    // As long as we have all users under the same organisation
    private readonly Guid TempOrganisationId = new("ae7113f0-1b52-40e5-9e77-5acb10e7fdad");

    private readonly DatabaseContext _database;

    public Guid OrganisationId => OrganisationIdAsync().Result;

    public OrganisationIdProvider(DatabaseContext database)
    {
        _database = database;
    }

    public async Task<Guid> OrganisationIdAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine(
            "### Started OrganisationIdProvider.OrganisationIdAsync on ContextId {0} Database {1}",
            _database.ContextId,
            _database.Database.GetDbConnection().Database);
        await _database.Organisations
            .Upsert(new Data.Entity.OrganisationEntity
            {
                Id = TempOrganisationId,
                Slug = "single-organisation"
            })
            .On(e => new { Id = e.Id })
            .RunAsync(cancellationToken);

        return TempOrganisationId;
    }
}
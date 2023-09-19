using App.Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace App;

public class AppHttpContext
{
    // As long as we have all users under the same organisation
    private readonly Guid TempOrganisationId = new("ae7113f0-1b52-40e5-9e77-5acb10e7fdad");

    private readonly DatabaseContext _database;

    public AppHttpContext(DatabaseContext database)
    {
        _database = database;
    }

    public async Task<Guid> OrganisationIdAsync(CancellationToken cancellationToken = default)
    {
        await _database.Organisations
            .Upsert(new Backend.Data.Entity.OrganisationEntity
            {
                Id = TempOrganisationId,
                Slug = "single-organisation"
            })
            .On(e => new { Id = e.Id })
            .RunAsync(cancellationToken);

        return TempOrganisationId;
    }

    public Guid OrganisationId()
    {
        return OrganisationIdAsync().Result;
    }
}
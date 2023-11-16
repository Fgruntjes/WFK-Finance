using App.Data;
using App.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace App;

public class OrganisationIdProvider
{
    // As long as we have all users under the same organisation
    private readonly Guid TempOrganisationId = new("ae7113f0-1b52-40e5-9e77-5acb10e7fdad");
    private readonly DatabaseContext _database;
    private bool _organisationStored = false;

    public Guid OrganisationId => OrganisationIdAsync().GetAwaiter().GetResult();

    public OrganisationIdProvider(DatabaseContext database)
    {
        _database = database;
    }

    public async Task<Guid> OrganisationIdAsync(CancellationToken cancellationToken = default)
    {
        if (_organisationStored)
        {
            return TempOrganisationId;
        }

        await _database.Organisations
            .Upsert(new OrganisationEntity
            {
                Id = TempOrganisationId,
                Slug = "only-organisation",
            })
            .On(i => new
            {
                i.Id,
            })
            .RunAsync(cancellationToken);
        await _database.SaveChangesAsync(cancellationToken);

        return TempOrganisationId;
    }
}
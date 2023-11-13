using App.Data;
using Microsoft.EntityFrameworkCore;

namespace App;

public class OrganisationIdProvider
{
    // As long as we have all users under the same organisation
    private readonly Guid TempOrganisationId = new("ae7113f0-1b52-40e5-9e77-5acb10e7fdad");

    public Guid OrganisationId => TempOrganisationId;

    public async Task<Guid> OrganisationIdAsync(CancellationToken cancellationToken = default)
    {
        return TempOrganisationId;
    }
}
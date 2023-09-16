using App.Backend.Data;
using App.Backend.Data.Entity;
using Microsoft.EntityFrameworkCore;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Backend.Service;

public class InstitutionConnectionCreateService
{
    private readonly DatabaseContext _database;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INordigenClient _nordigenClient;

    public InstitutionConnectionCreateService(
        DatabaseContext database,
        IHttpContextAccessor httpContextAccessor,
        INordigenClient nordigenClient)
    {
        _database = database;
        _httpContextAccessor = httpContextAccessor;
        _nordigenClient = nordigenClient;
    }

    public async Task<InstitutionConnectionEntity> Connect(Guid institutionId, Uri returnUrl, CancellationToken cancellationToken = default)
    {
        var institution = await _database.Institutions.FindAsync(institutionId)
            ?? throw new ArgumentOutOfRangeException(nameof(institutionId));

        var connectEntity = await GetConnectUrl(institution, cancellationToken);
        if (connectEntity != null)
        {
            return connectEntity;
        }

        var requisitionResponse = await _nordigenClient.Requisitions.Post(new RequisitionCreation(returnUrl, institution.ExternalId)
        {
            AccountSelection = true
        });

        connectEntity = await StoreConnectUrl(institutionId, requisitionResponse.Link, requisitionResponse.Id.ToString(), cancellationToken);
        return connectEntity;
    }

    private async Task<InstitutionConnectionEntity?> GetConnectUrl(InstitutionEntity institution, CancellationToken cancellationToken = default)
    {
        var organisationId = _httpContextAccessor.GetOrganisationId();

        return await _database.InstitutionConnections
            .AsQueryable()
            .Where(c => c.OrganisationId == organisationId && c.InstitutionId == institution.Id)
            .OrderBy(e => e.CreatedAt)
            .Take(1)
            .FirstOrDefaultAsync();
    }

    private async Task<InstitutionConnectionEntity> StoreConnectUrl(Guid institutionId, Uri connectUrl, string connectionId, CancellationToken cancellationToken = default)
    {
        var organisationId = _httpContextAccessor.GetOrganisationId();
        var connection = new InstitutionConnectionEntity
        {
            OrganisationId = organisationId,
            InstitutionId = institutionId,
            ConnectUrl = connectUrl,
            ExternalId = connectionId
        };

        var result = await _database.InstitutionConnections
            .Upsert(connection)
            .On(c => new { c.OrganisationId, c.InstitutionId })
            .WhenMatched(c => new InstitutionConnectionEntity
            {
                ConnectUrl = connectUrl,
            })
            .RunAsync();

        return connection;
    }
}
using App.Data;
using App.Data.Entity;
using Microsoft.EntityFrameworkCore;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Backend.Service;

public class InstitutionConnectionCreateService
{
    private readonly DatabaseContext _database;
    private readonly OrganisationIdProvider _organisationIdProvider;
    private readonly INordigenClient _nordigenClient;

    public InstitutionConnectionCreateService(
        DatabaseContext database,
        OrganisationIdProvider organisationIdProvider,
        INordigenClient nordigenClient)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
        _nordigenClient = nordigenClient;
    }

    public async Task<InstitutionConnectionEntity> Connect(Guid institutionId, Uri returnUrl, CancellationToken cancellationToken = default)
    {
        var institution = await _database.Institutions
            .FindAsync(institutionId)
            ?? throw new ArgumentOutOfRangeException(nameof(institutionId), institutionId, "Could not find institution");

        var connectEntity = await GetConnectUrl(institution, cancellationToken);
        if (connectEntity != null)
        {
            return connectEntity;
        }

        var requisitionResponse = await _nordigenClient.Requisitions.Post(new RequisitionCreation(returnUrl, institution.ExternalId)
        {
            AccountSelection = false
        });

        connectEntity = await StoreConnectUrl(institutionId, requisitionResponse.Link, requisitionResponse.Id.ToString(), cancellationToken);
        return connectEntity;
    }

    private async Task<InstitutionConnectionEntity?> GetConnectUrl(InstitutionEntity institution, CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.OrganisationId;

        return await _database.InstitutionConnections
            .AsQueryable()
            .Where(c => c.OrganisationId == organisationId && c.InstitutionId == institution.Id)
            .OrderBy(e => e.CreatedAt)
            .Take(1)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<InstitutionConnectionEntity> StoreConnectUrl(Guid institutionId, Uri connectUrl, string connectionId, CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.OrganisationId;
        var result = await _database.InstitutionConnections.AddAsync(new InstitutionConnectionEntity
        {
            OrganisationId = organisationId,
            InstitutionId = institutionId,
            ConnectUrl = connectUrl,
            ExternalId = connectionId
        }, cancellationToken);

        await _database.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }
}
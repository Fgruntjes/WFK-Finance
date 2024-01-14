using App.Institution.Exception;
using App.Lib.Data;
using App.Lib.Data.Entity;
using Microsoft.EntityFrameworkCore;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Institution.Service;

internal class InstitutionConnectionCreateService : IInstitutionConnectionCreateService
{
    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;
    private readonly INordigenClient _nordigenClient;

    public InstitutionConnectionCreateService(
        DatabaseContext database,
        IOrganisationIdProvider organisationIdProvider,
        INordigenClient nordigenClient)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
        _nordigenClient = nordigenClient;
    }

    public async Task<InstitutionConnectionEntity> Connect(Guid institutionId, Uri returnUrl, CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.GetOrganisationId();
        var institution = await _database.Institutions
            .FindAsync(new object[] { institutionId }, cancellationToken: cancellationToken)
            ?? throw new InstitutionNotFoundException(institutionId);

        var connectEntity = await GetConnectUrl(institution, organisationId, cancellationToken);
        if (connectEntity != null)
        {
            return connectEntity;
        }

        var requisitionResponse = await _nordigenClient.Requisitions.Post(new RequisitionCreation(returnUrl, institution.ExternalId)
        {
            AccountSelection = false
        });

        connectEntity = await StoreConnectUrl(
            organisationId,
            institutionId,
            requisitionResponse.Link,
            requisitionResponse.Id.ToString(),
            cancellationToken);
        return connectEntity;
    }

    private async Task<InstitutionConnectionEntity?> GetConnectUrl(
        InstitutionEntity institution,
        Guid organisationId,
        CancellationToken cancellationToken = default)
    {
        return await _database.InstitutionConnections
            .AsQueryable()
            .Where(c => c.OrganisationId == organisationId && c.InstitutionId == institution.Id)
            .OrderBy(e => e.CreatedAt)
            .Take(1)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<InstitutionConnectionEntity> StoreConnectUrl(
        Guid organisationId,
        Guid institutionId,
        Uri connectUrl,
        string connectionId,
        CancellationToken cancellationToken = default)
    {
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
using App.Backend.Auth;
using App.Backend.Data;
using App.Backend.Data.Entity;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Backend.Service;

public class InstitutionConnectionService
{
    private readonly DatabaseContext _databaseContext;
    private readonly AuthContext _authContext;
    private readonly INordigenClient _nordigenClient;
    private readonly InstitutionService _institutionService;

    public InstitutionConnectionService(
        DatabaseContext databaseContext,
        AuthContext authContext,
        INordigenClient nordigenClient,
        InstitutionService institutionService)
    {
        _databaseContext = databaseContext;
        _authContext = authContext;
        _nordigenClient = nordigenClient;
        _institutionService = institutionService;
    }

    public async Task<InstitutionConnectionEntity> Create(ObjectId institutionId, Uri returnUrl, CancellationToken cancellationToken = default)
    {
        var institution = await _institutionService.Get(institutionId, cancellationToken);
        var requisitionResponse = await _nordigenClient.Requisitions.Post(new RequisitionCreation(returnUrl, institution.ExternalId)
        {
            AccountSelection = true
        });

        return await PersistConnection(requisitionResponse, institution, cancellationToken);
    }

    public async Task<InstitutionConnectionEntity> Refresh(ObjectId id, CancellationToken cancellationToken = default)
    {
        var connection = await Get(id, cancellationToken);
        var institution = await _institutionService.Get(connection.InstitutionId, cancellationToken);
        var requisition = await _nordigenClient.Requisitions.Get(Guid.Parse(connection.ExternalConnectionId));

        return await PersistConnection(requisition, institution, cancellationToken);
    }

    public async Task<InstitutionConnectionEntity> Get(ObjectId id, CancellationToken cancellationToken = default)
    {
        var result = await _databaseContext.InstitutionConnections.FindAsync(
            c => c.Id == id,
            new() { Limit = 1 },
            cancellationToken);

        return await result.SingleAsync(cancellationToken);
    }

    private async Task<InstitutionConnectionEntity> PersistConnection(
        Requisition requisition,
        InstitutionEntity institution,
        CancellationToken cancellationToken = default)
    {
        var connection = new InstitutionConnectionEntity
        {
            TenantId = new ObjectId((await _authContext.GetTenant(cancellationToken)).Id),
            InstitutionId = institution.Id,
            ExternalConnectionId = requisition.Id.ToString(),
            ConnectUrl = requisition.Link,
            Accounts = requisition.Accounts
                .Select(async accountId =>
                {
                    var accountDetails = await _nordigenClient.Accounts.GetDetails(accountId);

                    return new InstitutionConnectionEntity.InstitutionAccount
                    {
                        ExternalId = accountId.ToString(),
                        OwnerName = accountDetails.OwnerName,
                        Iban = accountDetails.Iban
                    };
                })
                .Select(t => t.Result)
                .ToList()
        };

        var filter = Builders<InstitutionConnectionEntity>.Filter.And(
            Builders<InstitutionConnectionEntity>.Filter.Eq(c => c.TenantId, connection.TenantId),
            Builders<InstitutionConnectionEntity>.Filter.Eq(c => c.ExternalConnectionId, connection.ExternalConnectionId));

        var update = Builders<InstitutionConnectionEntity>.Update
            .Set(c => c.TenantId, connection.TenantId)
            .Set(c => c.ExternalConnectionId, connection.ExternalConnectionId)
            .Set(c => c.ConnectUrl, connection.ConnectUrl)
            .Set(c => c.Accounts, connection.Accounts)
            .Set(c => c.InstitutionId, connection.InstitutionId);

        return await _databaseContext.InstitutionConnections.FindOneAndUpdateAsync(
            filter,
            update,
            new FindOneAndUpdateOptions<InstitutionConnectionEntity>
            {
                IsUpsert = true
            },
            cancellationToken);
    }
}
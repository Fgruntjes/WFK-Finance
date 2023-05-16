using App.Backend.Auth;
using App.Backend.Data;
using App.Backend.Data.Entity;
using App.Backend.DTO;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Backend.Service;

public class BankConnectionService
{
    private readonly DatabaseContext _databaseContext;
    private readonly AuthContext _authContext;
    private readonly INordigenClient _nordigenClient;

    public BankConnectionService(
        DatabaseContext databaseContext,
        AuthContext authContext,
        INordigenClient nordigenClient)
    {
        _databaseContext = databaseContext;
        _authContext = authContext;
        _nordigenClient = nordigenClient;
    }

    public async Task<IEnumerable<BankConnection>> List(ListRequest request)
    {
        var tenant = await _authContext.GetTenant();
        return _databaseContext.BankConnections.AsQueryable()
            .Where(c => c.TenantId == new ObjectId(tenant.Id))
            .Skip(request.Start)
            .Take(request.Limit)
            .Select(c => new BankConnection
            {
                Id = c.Id.ToString(),
                ExternalId = c.ExternalId,
                Name = c.BankId,
                ConnectUrl = c.ConnectUrl
            });
    }

    public async Task<BankConnectResponse> Connect(BankConnectRequest request, CancellationToken cancellationToken = default)
    {
        var connectEntity = await GetConnectUrl(request.BankId, cancellationToken);
        if (connectEntity != null)
        {
            return new BankConnectResponse
            {
                Url = connectEntity.ConnectUrl
            };
        }

        var requisitionResponse = await _nordigenClient.Requisitions.Post(new RequisitionCreation(request.ReturnUrl, request.BankId)
        {
            AccountSelection = true
        });

        await StoreConnectUrl(request.BankId, requisitionResponse.Link, requisitionResponse.Id.ToString(), cancellationToken);
        return new BankConnectResponse
        {
            Url = requisitionResponse.Link
        };
    }

    private async Task<BankConnectionEntity?> GetConnectUrl(string bankId, CancellationToken cancellationToken = default)
    {
        var tenant = await _authContext.GetTenant(cancellationToken);

        var result = await _databaseContext.BankConnections.FindAsync(
            c => c.TenantId == new ObjectId(tenant.Id) && c.BankId == bankId,
            new() { Limit = 1 },
            cancellationToken);
        return await result.FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<BankConnectionEntity> StoreConnectUrl(string bankId, Uri connectUrl, string connectionId, CancellationToken cancellationToken = default)
    {
        var connection = new BankConnectionEntity
        {
            TenantId = new ObjectId((await _authContext.GetTenant(cancellationToken)).Id),
            BankId = bankId,
            ConnectUrl = connectUrl,
            ExternalId = connectionId
        };

        var filter = Builders<BankConnectionEntity>.Filter.And(
            Builders<BankConnectionEntity>.Filter.Eq(c => c.TenantId, connection.TenantId),
            Builders<BankConnectionEntity>.Filter.Eq(c => c.BankId, connection.BankId));
        var update = Builders<BankConnectionEntity>.Update
            .Set(c => c.ConnectUrl, connection.ConnectUrl)
            .Set(c => c.ExternalId, connection.ExternalId);
        await _databaseContext.BankConnections.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true }, cancellationToken);

        return connection;
    }
}
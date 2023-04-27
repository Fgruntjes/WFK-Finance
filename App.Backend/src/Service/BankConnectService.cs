using App.Backend.Authorization;
using App.Backend.Data;
using MongoDB.Driver;

namespace App.Backend.Service;

public class BankConnectService
{
    private readonly DatabaseContext _databaseContext;
    private readonly AuthContext _authContext;

    public BankConnectService(DatabaseContext databaseContext, AuthContext authContext)
    {
        _databaseContext = databaseContext;
        _authContext = authContext;
    }

    public async Task<BankConnection> StoreConnectUrl(string bankId, Uri connectUrl, CancellationToken cancellationToken = default)
    {
        var connection = new BankConnection
        {
            TenantId = _authContext.CurrentTenant,
            BankId = bankId,
            ConnectUrl = connectUrl
        };

        await _databaseContext.BankConnections.ReplaceOneAsync(
            c => c.TenantId == connection.TenantId && c.BankId == connection.BankId,
            connection,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);

        await _databaseContext.BankConnections.InsertOneAsync(connection, new InsertOneOptions(), cancellationToken);

        return connection;
    }
}
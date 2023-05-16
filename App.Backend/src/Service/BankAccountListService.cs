using App.Backend.Auth;
using App.Backend.Data;
using App.Backend.Data.Entity;
using App.Backend.DTO;
using MongoDB.Bson;
using MongoDB.Driver;
using VMelnalksnis.NordigenDotNet;

namespace App.Backend.Service;

public class BankAccountListService
{
    private readonly DatabaseContext _databaseContext;
    private readonly AuthContext _authContext;
    private readonly INordigenClient _nordigenClient;

    public BankAccountListService(
        DatabaseContext databaseContext,
        AuthContext authContext,
        INordigenClient nordigenClient)
    {
        _databaseContext = databaseContext;
        _authContext = authContext;
        _nordigenClient = nordigenClient;
    }

    public async Task<IEnumerable<BankAccount>> List(CancellationToken cancellationToken = default)
    {
        var currentTenant = await _authContext.GetTenant();
        var accountsCursor = await _databaseContext.BankAccounts.FindAsync(
            Builders<BankAccountEntity>.Filter.Eq(c => c.Connection.TenantId, new ObjectId(currentTenant.Id)),
            cancellationToken: cancellationToken);

        return (await accountsCursor.ToListAsync())
            .Select(account => new BankAccount
            {
                Id = account.Id.ToString(),
                AccountName = account.AccountName,
                AccountNumber = account.AccountNumber,
                BankName = account.BankName,
            });
    }

    public async Task Refresh(CancellationToken cancellationToken = default)
    {
        var currentTenant = await _authContext.GetTenant();
        var connectionsCursor = await _databaseContext.InstitutionConnections.FindAsync(
            Builders<InstitutionConnectionEntity>.Filter.Eq(c => c.TenantId, new ObjectId(currentTenant.Id)),
            cancellationToken: cancellationToken);
        var connectionsList = await connectionsCursor.ToListAsync(cancellationToken);

        await Task.WhenAll(connectionsList.Select(async connection =>
        {
            var connectionAccounts = await _nordigenClient.Requisitions.Get(Guid.Parse(connection.ExternalId));
            connectionAccounts.Accounts.ForEach(async account =>
            {
                var accountInfo = await _nordigenClient.Accounts.Get(account);
                await Upsert(
                    new BankAccountEntity
                    {
                        Connection = connection,
                        ExternalId = accountInfo.Id.ToString(),
                        AccountNumber = accountInfo.Iban,
                        BankName = accountInfo.InstitutionId,
                    },
                    connection);
            });
        }));
    }

    private async Task Upsert(BankAccountEntity accountInfo, InstitutionConnectionEntity connection)
    {
        var filter = Builders<BankAccountEntity>.Filter.And(
            Builders<BankAccountEntity>.Filter.Eq(c => c.Connection.TenantId, connection.TenantId),
            Builders<BankAccountEntity>.Filter.Eq(c => c.ExternalId, accountInfo.ExternalId));
        var update = Builders<BankAccountEntity>.Update
            .Set(c => c.AccountName, accountInfo.AccountName)
            .Set(c => c.AccountNumber, accountInfo.AccountNumber)
            .Set(c => c.BankName, accountInfo.BankName);
        await _databaseContext.BankAccounts.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }
}
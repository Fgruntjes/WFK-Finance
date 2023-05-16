using App.Backend.Data;
using App.Backend.Data.Entity;
using MongoDB.Driver;

namespace App.Backend.Service;

public class UserService
{
    private readonly DatabaseContext _databaseContext;

    public UserService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<UserEntity> GetOrCreate(string externalId, CancellationToken cancellationToken = default)
    {
        return await _databaseContext.Users.FindOrCreateAsync(
            Builders<UserEntity>.Filter.Eq(user => user.ExternalId, externalId),
            Builders<UserEntity>.Update.Set(user => user.ExternalId, externalId),
            cancellationToken);
    }
}
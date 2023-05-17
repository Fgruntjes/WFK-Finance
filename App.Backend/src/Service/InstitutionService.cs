using App.Backend.Data;
using App.Backend.Data.Entity;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using VMelnalksnis.NordigenDotNet;

namespace App.Backend.Service;

public class InstitutionService
{
    private readonly DatabaseContext _databaseContext;
    private readonly INordigenClient _nordigenClient;

    public InstitutionService(
        DatabaseContext databaseContext,
        INordigenClient nordigenClient)
    {
        _databaseContext = databaseContext;
        _nordigenClient = nordigenClient;
    }
    public async Task<InstitutionEntity> Get(ObjectId id, CancellationToken cancellationToken = default)
    {
        var result = await _databaseContext.Institutions.FindAsync(
            c => c.Id == id,
            new() { Limit = 1 },
            cancellationToken);

        return await result.SingleAsync(cancellationToken);
    }

    public async Task<InstitutionEntity> Get(string externalId, CancellationToken cancellationToken = default)
    {
        var result = await _databaseContext.Institutions.FindAsync(
            c => c.ExternalId == externalId,
            new() { Limit = 1 },
            cancellationToken);

        var entity = await result.FirstOrDefaultAsync(cancellationToken);
        if (entity != null)
        {
            return entity;
        }

        var institution = await _nordigenClient.Institutions.Get(externalId);
        entity = new InstitutionEntity
        {
            ExternalId = externalId,
            Name = institution.Name,
            Logo = institution.Logo
        };

        await _databaseContext.Institutions.InsertOneAsync(entity, cancellationToken: cancellationToken);
        return entity;
    }

    public async Task<IList<InstitutionEntity>> GetMany(IList<ObjectId> id, CancellationToken cancellationToken = default)
    {
        var result = await _databaseContext.Institutions.FindAsync(
        Builders<InstitutionEntity>.Filter.In(c => c.Id, id),
        cancellationToken: cancellationToken);

        return await result.ToListAsync(cancellationToken);
    }

    public async Task<IList<InstitutionEntity>> Search(string country, CancellationToken cancellationToken = default)
    {
        await UpdateInstitutes(country, cancellationToken);

        var result = await _databaseContext.Institutions.FindAsync(
            c => c.Countries.Contains(country),
            cancellationToken: cancellationToken);

        return await result.ToListAsync(cancellationToken);
    }

    private async Task UpdateInstitutes(string country, CancellationToken cancellationToken = default)
    {
        var institutionEntities = (await _nordigenClient.Institutions.GetByCountry(country))
            .Select(institution => new InstitutionEntity
            {
                ExternalId = institution.Id,
                Name = institution.Name,
                Logo = institution.Logo,
                Countries = institution.Countries,
            })
            .ToList();

        var institutionUpdates = institutionEntities.Select(institution =>
            {
                var filter = Builders<InstitutionEntity>.Filter.Eq(c => c.ExternalId, institution.ExternalId);
                var update = Builders<InstitutionEntity>.Update
                    .Set(c => c.ExternalId, institution.ExternalId)
                    .Set(c => c.Name, institution.Name)
                    .Set(c => c.Logo, institution.Logo)
                    .Set(c => c.Countries, institution.Countries);

                return new UpdateOneModel<InstitutionEntity>(filter, update)
                {
                    IsUpsert = true
                };
            });

        await _databaseContext.Institutions.BulkWriteAsync(institutionUpdates, cancellationToken: cancellationToken);
    }
}
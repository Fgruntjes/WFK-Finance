using App.Backend.Data;
using App.Backend.Data.Entity;
using Microsoft.EntityFrameworkCore;
using VMelnalksnis.NordigenDotNet;

namespace App.Backend.Service;

public class InstitutionSearchService
{
    private readonly DatabaseContext _database;
    private readonly INordigenClient _nordigenClient;

    public InstitutionSearchService(DatabaseContext database, INordigenClient nordigenClient)
    {
        _database = database;
        _nordigenClient = nordigenClient;
    }

    public async Task<IEnumerable<InstitutionEntity>> Search(string countryIso2, CancellationToken cancellationToken = default)
    {
        var institutions = await _database.Institutions
            .Where(i => i.CountryIso2 == countryIso2)
            .ToListAsync(cancellationToken);

        if (institutions.Count > 0)
        {
            return institutions;
        }

        return await Refresh(countryIso2, cancellationToken);
    }

    public async Task<IReadOnlyList<InstitutionEntity>> Refresh(string countryIso2, CancellationToken cancellationToken)
    {
        var institutions = (await _nordigenClient.Institutions.GetByCountry(countryIso2))
            .Select(i => new InstitutionEntity
            {
                ExternalId = i.Id,
                Name = i.Name,
                Logo = i.Logo,
                CountryIso2 = countryIso2,
            });

        await _database.Institutions
                .UpsertRange(institutions)
                .On(i => new
                {
                    i.ExternalId
                })
                .NoUpdate()
                .RunAsync();

        return new List<InstitutionEntity>(institutions);
    }
}
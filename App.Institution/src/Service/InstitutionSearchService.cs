using App.Institution.Interface;
using App.Lib.Data;
using App.Lib.Data.Entity;
using Microsoft.EntityFrameworkCore;
using VMelnalksnis.NordigenDotNet;

namespace App.Institution.Service;

internal class InstitutionSearchService : IInstitutionSearchService
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
        IList<InstitutionEntity> institutions = await _database.Institutions
            .Where(i => i.CountryIso2 == countryIso2)
            .ToListAsync(cancellationToken);

        if (institutions.Count > 0)
        {
            return institutions;
        }

        return await Refresh(countryIso2, cancellationToken);
    }

    public async Task<IList<InstitutionEntity>> Refresh(string countryIso2, CancellationToken cancellationToken)
    {
        var institutions = (await _nordigenClient.Institutions.GetByCountry(countryIso2, cancellationToken))
            .Select(i => new InstitutionEntity
            {
                ExternalId = i.Id,
                Name = i.Name,
                Logo = i.Logo,
                CountryIso2 = countryIso2,
            }).ToList();

        await _database.Institutions
                .UpsertRange(institutions)
                .On(i => new
                {
                    i.ExternalId
                })
                .NoUpdate()
                .RunAsync(cancellationToken);

        return new List<InstitutionEntity>(institutions.ToList());
    }
}
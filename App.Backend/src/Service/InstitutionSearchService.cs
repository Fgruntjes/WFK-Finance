using App.Data;
using App.Data.Entity;
using Microsoft.EntityFrameworkCore;
using VMelnalksnis.NordigenDotNet;

namespace App.Backend.Service;

public class InstitutionSearchService
{
    private const string _nordigenTestInstitutionId = "SANDBOXFINANCE_SFIN0000";
    private readonly DatabaseContext _database;
    private readonly INordigenClient _nordigenClient;
    private readonly IHostEnvironment _environment;

    public InstitutionSearchService(DatabaseContext database, INordigenClient nordigenClient, IHostEnvironment environment)
    {
        _database = database;
        _nordigenClient = nordigenClient;
        _environment = environment;
    }

    public async Task<IEnumerable<InstitutionEntity>> Search(string countryIso2, CancellationToken cancellationToken = default)
    {
        IList<InstitutionEntity> institutions = await _database.Institutions
            .Where(i => i.CountryIso2 == countryIso2)
            .ToListAsync(cancellationToken);

        var minInstitutions = _environment.IsProduction() ? 0 : 1;
        if (institutions.Count > minInstitutions)
        {
            return institutions;
        }

        institutions = await Refresh(countryIso2, cancellationToken);
        if (!_environment.IsProduction())
        {
            await EnsureTestInstitution(institutions, countryIso2, cancellationToken);
        }
        return institutions;
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
            });

        await _database.Institutions
                .UpsertRange(institutions)
                .On(i => new
                {
                    i.ExternalId
                })
                .NoUpdate()
                .RunAsync(cancellationToken);

        return new List<InstitutionEntity>(institutions);
    }

    private async Task EnsureTestInstitution(IList<InstitutionEntity> institutions, string countryIso2, CancellationToken cancellationToken)
    {
        if (institutions.Any(institutions => institutions.ExternalId == _nordigenTestInstitutionId))
        {
            return;
        }

        var entity = new InstitutionEntity
        {
            ExternalId = _nordigenTestInstitutionId,
            Name = "TEST_INSTITUTION",
            //Logo = "https://cdn.nordigen.com/logos/sandboxfinance.png",
            CountryIso2 = countryIso2,
        };

        await _database.Institutions
            .Upsert(entity)
            .On(i => new
            {
                i.ExternalId
            })
            .NoUpdate()
            .RunAsync(cancellationToken);

        institutions.Add(entity);
    }
}
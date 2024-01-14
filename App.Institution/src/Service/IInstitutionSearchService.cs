using App.Lib.Data.Entity;

namespace App.Institution.Service;

public interface IInstitutionSearchService
{
    public Task<IEnumerable<InstitutionEntity>> Search(string countryIso2, CancellationToken cancellationToken = default);
}
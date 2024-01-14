using App.Lib.Data.Entity;

namespace App.Institution.Service;

public interface IInstitutionConnectionCreateService
{
    public Task<InstitutionConnectionEntity> Connect(
        Guid institutionId,
        Uri returnUrl,
        CancellationToken cancellationToken = default);
}
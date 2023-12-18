using App.Lib.Data.Entity;

namespace App.Lib.InstitutionConnection.Service;

public interface IInstitutionConnectionCreateService
{
    public Task<InstitutionConnectionEntity> Connect(
        Guid institutionId,
        Uri returnUrl,
        CancellationToken cancellationToken = default);
}
using App.Backend.Data.Entity;

namespace App.Backend.DTO;

public class InstitutionConnection : IEntityConvertable<InstitutionConnectionEntity, InstitutionConnection>
{
    public class Account
    {
        public string? OwnerName { get; init; }
        public string? Iban { get; init; } = null!;
    }

    public string Id { get; init; } = null!;
    public string InstitutionId { get; set; } = null!;
    public Uri ConnectUrl { get; set; } = null!;
    public IList<Account> Accounts { get; set; } = null!;

    public static InstitutionConnection FromEntity(InstitutionConnectionEntity entity)
    {
        return new InstitutionConnection
        {
            Id = entity.Id.ToString(),
            InstitutionId = entity.InstitutionId.ToString(),
            ConnectUrl = entity.ConnectUrl,
            Accounts = entity.Accounts.Select(a => new Account
            {
                OwnerName = a.OwnerName,
                Iban = a.Iban
            }).ToList()
        };
    }
}

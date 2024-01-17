using App.Lib.Data.Entity;
using Gridify;

namespace App.Backend.Gridify;

class InstitutionTransactionQueryMapper : GridifyMapper<InstitutionAccountTransactionEntity>
{
    public InstitutionTransactionQueryMapper()
    {
        GenerateMappings();
        AddMap("InstitutionId", e => e.Account.InstitutionConnection.InstitutionId);
        AddMap("AccountIban", e => e.Account.Iban);
    }
}
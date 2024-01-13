using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public static class InstitutionAccountTransactionMapper
{
    public static InstitutionAccountTransaction ToDto(this InstitutionAccountTransactionEntity entity)
    {
        return new InstitutionAccountTransaction
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt.ToDateTimeUtc(),
            AccountId = entity.AccountId,
            ExternalId = entity.ExternalId,
            UnstructuredInformation = entity.UnstructuredInformation,
            TransactionCode = entity.TransactionCode,
            CounterPartyName = entity.CounterPartyName,
            CounterPartyAccount = entity.CounterPartyAccount,
            Amount = entity.Amount,
            Currency = entity.Currency,
            Date = entity.Date.ToDateTimeUtc()
        };
    }
}
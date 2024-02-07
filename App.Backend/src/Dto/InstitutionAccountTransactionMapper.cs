using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public static class InstitutionAccountTransactionMapper
{
    public static InstitutionTransactionDto ToDto(this InstitutionAccountTransactionEntity entity)
    {
        return new InstitutionTransactionDto
        {
            Id = entity.Id,
            AccountId = entity.AccountId,
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
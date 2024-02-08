using App.Lib.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Dto;

public static class InstitutionTransactionMapper
{
    public static async Task<ICollection<InstitutionTransactionDto>> ToDto(
        this IQueryable<InstitutionAccountTransactionEntity> query,
        CancellationToken cancellationToken = default)
    {
        return await query
            .Include(e => e.Account)
            .Include(e => e.Account.InstitutionConnection)
            .Select(e => e.ToDto())
            .ToListAsync(cancellationToken);
    }

    public static ICollection<InstitutionTransactionDto> ToDto(this ICollection<InstitutionAccountTransactionEntity> entities)
    {
        return entities
            .Select(e => e.ToDto())
            .ToList();
    }

    public static InstitutionTransactionDto ToDto(this InstitutionAccountTransactionEntity entity)
    {
        return new InstitutionTransactionDto
        {
            Id = entity.Id,
            AccountIban = entity.Account.Iban,
            InstitutionId = entity.Account.InstitutionConnection.InstitutionId,
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
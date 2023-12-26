using App.Lib.Data;
using App.Lib.Data.Entity;
using App.Lib.InstitutionConnection.Exception;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using VMelnalksnis.NordigenDotNet.Accounts;

namespace App.Lib.InstitutionConnection.Service;

internal class TransactionImportService : ITransactionImportService
{
    private readonly IAccountClient _accountClient;
    private readonly DatabaseContext _database;
    private readonly IClock _clock;
    private readonly ILogger<TransactionImportService> _logger;

    public TransactionImportService(
        ILoggerFactory loggerFactory,
        IAccountClient accountClient,
        DatabaseContext databaseContext,
        IClock clock)
    {
        _accountClient = accountClient;
        _database = databaseContext;
        _clock = clock;
        _logger = loggerFactory.CreateLogger<TransactionImportService>();
    }

    public async Task ImportAsync(Guid institutionAccountId, CancellationToken cancellationToken = default)
    {
        var lastDate = SystemClock.Instance.GetCurrentInstant();

        await ImportAsync(institutionAccountId, lastDate, cancellationToken);
    }

    public async Task ImportAsync(Guid institutionAccountId, Instant lastDate, CancellationToken cancellationToken = default)
    {
        var entity = await GetInstitutionAccountEntity(institutionAccountId, cancellationToken);

        // In some cases transactions get added with a delay
        var firstDate = (entity.LastImport ?? lastDate) - Duration.FromDays(30);
        await ImportAsync(entity, firstDate, lastDate, cancellationToken);
    }

    public async Task ImportAsync(
        Guid institutionAccountId,
        Instant firstDate,
        Instant lastDate,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetInstitutionAccountEntity(institutionAccountId, cancellationToken);
        await ImportAsync(entity, firstDate, lastDate, cancellationToken);
    }

    private async Task ImportAsync(
        InstitutionAccountEntity entity,
        Instant firstDate,
        Instant lastDate,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Started import for {AccountId} {AccountExternalId}",
            entity.Id,
            entity.ExternalId);

        entity.ImportStatus = ImportStatus.Working;
        await _database.SaveChangesAsync(cancellationToken);

        try
        {
            var externalTransactions = await GetExternalTransactions(
                entity,
                firstDate,
                lastDate,
                cancellationToken);

            var existingTransactions = await _database.InstitutionAccountTransactions
                .Where(t => t.AccountId == entity.Id)
                .Where(t => t.Date >= firstDate.ToStartOfTheDay())
                .Where(t => t.Date <= lastDate.ToEndOfTheDay())
                .ToDictionaryAsync(t => t.ExternalId, cancellationToken);

            var insertCount = 0;
            var updateCount = 0;
            var deleteCount = 0;
            foreach (var transaction in externalTransactions.Values)
            {
                if (existingTransactions.TryGetValue(transaction.ExternalId, out var existingTransaction))
                {
                    existingTransaction.Amount = transaction.Amount;
                    existingTransaction.Currency = transaction.Currency;
                    existingTransaction.Date = transaction.Date;
                    existingTransaction.TransactionCode = transaction.TransactionCode;
                    existingTransaction.UnstructuredInformation = transaction.UnstructuredInformation;
                    existingTransaction.CounterPartyName = transaction.CounterPartyName;
                    existingTransaction.CounterPartyAccount = transaction.CounterPartyAccount;

                    _logger.LogDebug(
                        "Update {AccountId} {AccountExternalId}, transaction {TransactionId} {TransactionExternalId}",
                        entity.Id,
                        entity.ExternalId,
                        existingTransaction.Id,
                        existingTransaction.ExternalId);
                    updateCount++;
                }
                else
                {
                    _logger.LogDebug(
                        "Insert {AccountId} {AccountExternalId}, transaction {TransactionId} {TransactionExternalId}",
                        entity.Id,
                        entity.ExternalId,
                        transaction.Id,
                        transaction.ExternalId);
                    _database.InstitutionAccountTransactions.Add(transaction);
                    insertCount++;
                }
            }

            // Remove accounts that are not in the new list
            var removeTransactions = existingTransactions.Values
                .Where(t => !externalTransactions.ContainsKey(t.ExternalId))
                .ToList();
            foreach (var removeTransaction in removeTransactions)
            {
                _logger.LogDebug(
                    "Delete {AccountId} {AccountExternalId}, transaction {TransactionId} {TransactionExternalId}",
                    entity.Id,
                    entity.ExternalId,
                    removeTransaction.Id,
                    removeTransaction.ExternalId);
                _database.InstitutionAccountTransactions.Remove(removeTransaction);
                deleteCount++;
            }

            entity.ImportStatus = ImportStatus.Success;
            entity.LastImport = _clock.GetCurrentInstant();
            entity.LastImportError = null;
            await _database.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
                "Finished import for {AccountId} {AccountExternalId}, inserted {InsertCount}, updated {UpdateCount} and deleted {DeleteCount}",
                entity.Id,
                entity.ExternalId,
                insertCount,
                updateCount,
                deleteCount);
        }
        catch (System.Exception exception)
        {
            entity.ImportStatus = ImportStatus.Failed;
            entity.LastImportError = exception.Message;
            _logger.LogCritical(
                exception,
                "Import for {AccountId} {AccountExternalId}, failed",
                entity.Id,
                entity.ExternalId);

            await _database.SaveChangesAsync(cancellationToken);
            throw;
        }

    }

    private async Task<IDictionary<string, InstitutionAccountTransactionEntity>> GetExternalTransactions(
        InstitutionAccountEntity entity,
        Instant firstDate,
        Instant lastDate,
        CancellationToken cancellationToken = default)
    {
        var response = await _accountClient.GetTransactions(
            new Guid(entity.ExternalId),
            new Interval(firstDate, lastDate),
            cancellationToken);

        return response.Booked
            .Select(t => new InstitutionAccountTransactionEntity
            {
                Account = entity,
                AccountId = entity.Id,
                Amount = t.TransactionAmount.Amount,
                Currency = t.TransactionAmount.Currency,
                Date = (t.ValueDate ?? t.BookingDate).ToUtc(),
                ExternalId = t.TransactionId,
                TransactionCode = t.BankTransactionCode,
                UnstructuredInformation = t.UnstructuredInformation,
            }).ToDictionary(e => e.ExternalId);

    }

    private async Task<InstitutionAccountEntity> GetInstitutionAccountEntity(Guid institutionAccountId, CancellationToken cancellationToken)
    {
        var entity = await _database.InstitutionAccounts.FindAsync(
            new object?[] { institutionAccountId },
            cancellationToken: cancellationToken);
        return entity ?? throw new InstitutionAccountNotFoundException(institutionAccountId);
    }
}
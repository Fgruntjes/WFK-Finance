using App.Lib.Data;
using App.Lib.Data.Entity;
using App.Lib.Data.Exception;
using App.TransactionCategory.Interface;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;

namespace App.TransactionCategory.Service;

class SimilarTransactionService : ISimilarTransactionService
{
    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;

    public SimilarTransactionService(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    public async Task<ICollection<InstitutionAccountTransactionEntity>> Find(Guid id, CancellationToken cancellationToken = default)
    {
        var OrganisationId = _organisationIdProvider.GetOrganisationId();
        var originalTransaction = await _database.InstitutionAccountTransactions
            .Where(e => e.Account.InstitutionConnection.OrganisationId == OrganisationId)
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(id);

        var transactionQuery = _database.InstitutionAccountTransactions
            .AsNoTracking()
            .Include(e => e.Account)
            .Include(e => e.Account.InstitutionConnection)
            .Where(e => e.Account.InstitutionConnection.OrganisationId == OrganisationId)
            .Where(e => e.CategoryId == null)
            .Where(e => e.Id != id)
            .AsQueryable();

        var result = new List<InstitutionAccountTransactionEntity>();
        await transactionQuery.ForEachAsync(compareTransaction =>
        {
            if (IsSimilar(originalTransaction, compareTransaction))
            {
                result.Add(compareTransaction);
            }
        }, cancellationToken);

        return result;
    }

    private bool IsSimilar(InstitutionAccountTransactionEntity originalTransaction, InstitutionAccountTransactionEntity compareTransaction)
    {
        if (StringSimilar(originalTransaction.CounterPartyAccount, compareTransaction.CounterPartyAccount))
        {
            return true;
        }

        if (StringSimilar(originalTransaction.CounterPartyName, compareTransaction.CounterPartyName))
        {
            return true;
        }

        if (StringSimilar(originalTransaction.UnstructuredInformation, compareTransaction.UnstructuredInformation))
        {
            return true;
        }

        return false;
    }

    private bool StringSimilar(string? a, string? b)
    {
        if (a == null || b == null)
        {
            return false;
        }

        return Fuzz.PartialRatio(a, b) > 80;
    }
}
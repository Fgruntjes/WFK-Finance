using App.Lib.Data.Entity;
using App.Lib.Data.Exception;
using App.Lib.Test.Data;
using App.Lib.Test.Database;
using App.TransactionCategory.Interface;
using App.TransactionCategory.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.TransactionCategory.Test.Service;

public class SimilarTransactionServiceTest
{
    private readonly DatabasePool _databasePool;
    private readonly ILoggerProvider _loggerProvider;

    public SimilarTransactionServiceTest(DatabasePool databasePool, ILoggerProvider loggerProvider)
    {
        _databasePool = databasePool;
        _loggerProvider = loggerProvider;
    }

    [Fact]
    public void ImplementationType()
    {
        // Act
        var fixture = new SimilarTransactionFixture(_databasePool, _loggerProvider);

        // Act
        var service = fixture.Services.GetRequiredService<ISimilarTransactionService>();

        // Assert
        service.Should().BeOfType<SimilarTransactionService>();
    }

    [Fact]
    public async Task Find_Similar()
    {
        // Arrange
        var fixture = new SimilarTransactionFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ISimilarTransactionService>();

        var transactionEntity = new FakeInstitutionAccountTransactionBuilder(fixture.AccountId)
            .WithDefaults(entity =>
            {
                entity.CounterPartyAccount = "123";
                entity.CounterPartyName = "SomeCompany";
                entity.UnstructuredInformation = "CCV*SPA SEREEN TIENHOVEN UT NLD<br>Pasvolgnr: 900 12-03-2025 11:13<br>Transactie: B78679 Term: CT3567";
            }).Generate();

        var transactions = new List<InstitutionAccountTransactionEntity>() {
            // Match, CounterPartyAccount
            new FakeInstitutionAccountTransactionBuilder(fixture.AccountId)
                .WithDefaults(entity => {
                    entity.Id = new Guid("60b6d920-9c9a-4947-ab2e-754582e3a673");
                    entity.CounterPartyAccount = "123";
                })
                .Generate(),

            // Match, CounterPartyName
            new FakeInstitutionAccountTransactionBuilder(fixture.AccountId)
                .WithDefaults(entity => {
                    entity.Id = new Guid("a7f9d8d0-0fcc-4d3e-a316-d7d136a714d7");
                    entity.CounterPartyName = "SomeCompany";
                })
                .Generate(),

            // Match, UnstructuredInformation
            new FakeInstitutionAccountTransactionBuilder(fixture.AccountId)
                .WithDefaults(entity => {
                    entity.Id = new Guid("16f7e2a4-52c4-4c86-aa57-53bd75d5b549");
                    entity.UnstructuredInformation = "CCV*SPA SEREEN TIENHOVEN UT NLD<br>Pasvolgnr: 900 04-02-2024 21:25<br>Transactie: A80045 Term: CT410018<br>Apple Pay<br>Valutadatum: 05-02-2024";
                })
                .Generate(),

            // Missmatch
            new FakeInstitutionAccountTransactionBuilder(fixture.AccountId)
                .WithDefaults(entity => entity.CounterPartyAccount = "456")
                .Generate(),

            // Missmatch, different organisation
            new FakeInstitutionAccountTransactionBuilder(fixture.AltAccountId)
                .WithDefaults(entity => entity.CounterPartyAccount = "123")
                .Generate(),
        };
        fixture.Database.SeedData(context =>
        {
            context.Add(transactionEntity);
            context.AddRange(transactions);
        });

        // Act
        var result = await service.Find(transactionEntity.Id);

        // Assert
        result.Should().BeEquivalentTo(
            new List<InstitutionAccountTransactionEntity>() {
                new() { Id = new Guid("60b6d920-9c9a-4947-ab2e-754582e3a673") },
                new() { Id = new Guid("a7f9d8d0-0fcc-4d3e-a316-d7d136a714d7") },
                new() { Id = new Guid("16f7e2a4-52c4-4c86-aa57-53bd75d5b549") },
            },
            o => o.Including(e => e.Id));
    }

    [Fact]
    public async Task Find_NotFound_DoesNotExist()
    {
        // Act
        var fixture = new SimilarTransactionFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ISimilarTransactionService>();

        // Act
        var act = async () => await service.Find(new Guid("535999bf-ff32-4e41-be02-93aa3320e85a"));

        // Assert
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(act);
        result.Data["Id"].Should().Be("535999bf-ff32-4e41-be02-93aa3320e85a");
    }

    [Fact]
    public async Task Find_NotFound_OutsideOrganisation()
    {
        // Act
        var fixture = new SimilarTransactionFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ISimilarTransactionService>();
        var transactionEntity = new FakeInstitutionAccountTransactionBuilder(fixture.AltAccountId).Generate();
        fixture.Database.SeedData(context => context.Add(transactionEntity));

        // Act
        var act = async () => await service.Find(transactionEntity.Id);

        // Assert
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(act);
        result.Data["Id"].Should().Be(transactionEntity.Id.ToString());
    }
}
using App.Lib.Data.Entity;
using App.Institution.Exception;
using App.Institution.Interface;
using App.Institution.Service;
using App.Lib.Test;
using App.Lib.Test.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using VMelnalksnis.NordigenDotNet.Accounts;
using Xunit.Sdk;

namespace App.Institution.Test.Service;

public class TransactionImportServiceTest
{
    private readonly DatabasePool _databasePool;
    private readonly ILoggerProvider _loggerProvider;

    public TransactionImportServiceTest(DatabasePool databasePool, ILoggerProvider loggerProvider)
    {
        _databasePool = databasePool;
        _loggerProvider = loggerProvider;
    }

    [Fact]
    public void Implementation()
    {
        // Arrange
        var fixture = new TransactionImportFixture(_databasePool, _loggerProvider);

        // Act
        var service = fixture.Services.GetRequiredService<ITransactionImportService>();

        // Assert
        service.Should().BeOfType<TransactionImportService>();
    }

    [Fact]
    public async Task AccountNotFoundException()
    {
        // Arrange
        var fixture = new TransactionImportFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ITransactionImportService>();
        var institutionId = new Guid("8f570488-eec8-45f3-96ae-bdc718ed453c");

        // Act
        var act = async () => await service.ImportAsync(institutionId);

        // Assert
        var exception = await Assert.ThrowsAsync<InstitutionAccountNotFoundException>(act);
        exception.Data["InstitutionAccountId"].Should().Be(institutionId);
    }

    [Fact]
    public async Task ImportNew()
    {
        // Arrange
        var fixture = new TransactionImportFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ITransactionImportService>();
        var importStart = Instant.FromUtc(2023, 07, 1, 12, 13, 00);
        var importEnd = Instant.FromUtc(2023, 10, 1, 12, 13, 00);

        fixture.Services.WithMock<IAccountClient>(mock =>
        {
            mock.Setup(r => r.GetTransactions(
                    new Guid(fixture.InstitutionAccountEntity.ExternalId),
                    It.Is<Interval>(v => v.Start == importStart && v.End == importEnd),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Transactions()
                {
                    Booked = new List<BookedTransaction>()
                    {
                        new ()
                        {
                            TransactionAmount = new AmountInCurrency()
                            {
                                Amount = (decimal) -10.0,
                                Currency = "EUR"
                            },
                            ValueDate = new LocalDate(2023, 07, 1),
                            TransactionId = "010304283669457170000000",
                            BankTransactionCode = null,
                            UnstructuredInformation = "UnstructuredInformation info 1",
                        },
                        new ()
                        {
                            TransactionAmount = new AmountInCurrency()
                            {
                                Amount = (decimal) -10.0,
                                Currency = "EUR"
                            },
                            BookingDate = new LocalDate(2023, 07, 3),
                            TransactionId = "010304288352786100000000",
                            BankTransactionCode = "CODE1",
                            UnstructuredInformation = "UnstructuredInformation info 2",
                        }
                    },
                    Pending = new List<PendingTransaction>()
                });
        });

        // Act
        await service.ImportAsync(fixture.InstitutionAccountEntity.Id, importStart, importEnd);

        // Assert
        fixture.Database.WithData(context =>
        {
            context.InstitutionAccountTransactions
                .Where(t => t.AccountId == fixture.InstitutionAccountEntity.Id)
                .ToList()
                .Should()
                .BeEquivalentTo(new List<InstitutionAccountTransactionEntity>()
                {
                    new ()
                    {
                        AccountId = fixture.InstitutionAccountEntity.Id,
                        Amount = -10,
                        Currency = "EUR",
                        Date = new LocalDate(2023, 07, 1).AtMidnight().InUtc().ToInstant(),
                        ExternalId = "010304283669457170000000",
                        TransactionCode = null,
                        UnstructuredInformation = "UnstructuredInformation info 1",
                    },
                    new ()
                    {
                        AccountId = fixture.InstitutionAccountEntity.Id,
                        Amount = -10,
                        Currency = "EUR",
                        Date = new LocalDate(2023, 07, 3).AtMidnight().InUtc().ToInstant(),
                        ExternalId = "010304288352786100000000",
                        TransactionCode = "CODE1",
                        UnstructuredInformation = "UnstructuredInformation info 2",
                    }
                }, options => options
                    .Excluding(e => e.Id)
                    .Excluding(e => e.CreatedAt));
        });
    }

    [Fact]
    public async Task ImportUpdate()
    {
        // Arrange
        var fixture = new TransactionImportFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ITransactionImportService>();

        fixture.Services.WithMock<IAccountClient>(mock =>
        {
            var transactionListFirst = new List<BookedTransaction>()
            {
                new()
                {
                    TransactionAmount = new AmountInCurrency()
                    {
                        Amount = (decimal)-10.0,
                        Currency = "EUR"
                    },
                    ValueDate = new LocalDate(2023, 07, 5),
                    TransactionId = "010304283669457170000001",
                    BankTransactionCode = null,
                    UnstructuredInformation = "UnstructuredInformation info 5",
                },
                new()
                {
                    TransactionAmount = new AmountInCurrency()
                    {
                        Amount = (decimal)-10.0,
                        Currency = "EUR"
                    },
                    BookingDate = new LocalDate(2023, 07, 3),
                    TransactionId = "010304288352786100000000",
                    BankTransactionCode = "CODE1",
                    UnstructuredInformation = "UnstructuredInformation info 2",
                }
            };
            var transactionListSecond = transactionListFirst.ToList();
            transactionListSecond[1].ValueDate = new LocalDate(2023, 07, 4);
            transactionListSecond[1].BankTransactionCode = "CODE2";
            transactionListSecond[1].UnstructuredInformation = "UnstructuredInformation info 3";

            mock.SetupSequence(r => r.GetTransactions(
                    It.IsAny<Guid>(),
                    It.IsAny<Interval>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Transactions { Booked = transactionListFirst, Pending = new List<PendingTransaction>() })
                .ReturnsAsync(new Transactions { Booked = transactionListSecond, Pending = new List<PendingTransaction>() });
        });

        // Act
        await service.ImportAsync(fixture.InstitutionAccountEntity.Id);
        await service.ImportAsync(fixture.InstitutionAccountEntity.Id);

        // Assert
        fixture.Database.WithData(context =>
        {
            context.InstitutionAccountTransactions
                .Where(t => t.AccountId == fixture.InstitutionAccountEntity.Id)
                .ToList()
                .Should()
                .BeEquivalentTo(new List<InstitutionAccountTransactionEntity>()
                {
                    new ()
                    {
                        AccountId = fixture.InstitutionAccountEntity.Id,
                        Amount = -10,
                        Currency = "EUR",
                        Date = new LocalDate(2023, 07, 5).AtMidnight().InUtc().ToInstant(),
                        ExternalId = "010304283669457170000001",
                        TransactionCode = null,
                        UnstructuredInformation = "UnstructuredInformation info 5",
                    },
                    new ()
                    {
                        AccountId = fixture.InstitutionAccountEntity.Id,
                        Amount = -10,
                        Currency = "EUR",
                        Date = new LocalDate(2023, 07, 4).AtMidnight().InUtc().ToInstant(),
                        ExternalId = "010304288352786100000000",
                        TransactionCode = "CODE2",
                        UnstructuredInformation = "UnstructuredInformation info 3",
                    }
                }, options => options
                    .Excluding(e => e.Id)
                    .Excluding(e => e.CreatedAt));
        });
    }

    [Fact]
    public async Task ImportStatusFlow()
    {
        // Arrange
        var fixture = new TransactionImportFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ITransactionImportService>();
        var now = Instant.FromUtc(2023, 07, 1, 12, 13, 00);

        fixture.Services.WithMock<IClock>(mock =>
        {
            mock.Setup(s => s.GetCurrentInstant())
                .Returns(now);
        });
        fixture.Services.WithMock<IAccountClient>(mock =>
        {
            mock.Setup(r => r.GetTransactions(
                    It.IsAny<Guid>(),
                    It.IsAny<Interval>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Transactions { Booked = new List<BookedTransaction>(), Pending = new List<PendingTransaction>() })
                .Callback(() =>
                {
                    // Assert queued
                    WithAccountEntity(fixture, entity => entity.ImportStatus.Should().Be(ImportStatus.Working));
                });
        });

        // Act
        await service.ImportAsync(fixture.InstitutionAccountEntity.Id);

        // Assert
        WithAccountEntity(fixture, entity =>
        {
            entity.LastImportError.Should().BeNull();
            entity.ImportStatus.Should().Be(ImportStatus.Success);
            entity.LastImport.Should().Be(now);
        });
    }

    [Fact]
    public async Task ImportStatusResetLastError()
    {
        // Arrange
        var fixture = new TransactionImportFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ITransactionImportService>();

        fixture.Services.WithMock<IAccountClient>(mock =>
        {
            mock.Setup(r => r.GetTransactions(
                    It.IsAny<Guid>(),
                    It.IsAny<Interval>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Transactions { Booked = new List<BookedTransaction>(), Pending = new List<PendingTransaction>() });
        });

        fixture.Database.SeedData(context =>
        {
            context.InstitutionAccounts.Find(fixture.InstitutionAccountEntity.Id)!
                .LastImportError = "Some Error";
        });

        // Act
        await service.ImportAsync(fixture.InstitutionAccountEntity.Id);

        // Assert
        WithAccountEntity(fixture, entity =>
        {
            entity.LastImportError.Should().BeNull();
        });
    }

    [Fact]
    public async Task ImportFailed()
    {
        // Arrange
        var fixture = new TransactionImportFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ITransactionImportService>();
        fixture.Services.WithMock<IAccountClient>(mock =>
        {
            mock.Setup(r => r.GetTransactions(
                    It.IsAny<Guid>(),
                    It.IsAny<Interval>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new System.Exception("Some Error"));
        });

        // Act
        var act = async () => await service.ImportAsync(fixture.InstitutionAccountEntity.Id);

        // Assert
        var exception = await Assert.ThrowsAsync<System.Exception>(act);
        exception.Message.Should().Be("Some Error");
        WithAccountEntity(fixture, entity =>
        {
            entity.LastImportError.Should().Be("Some Error");
        });
    }

    private static void WithAccountEntity(TransactionImportFixture fixture, Action<InstitutionAccountEntity> callback)
    {
        fixture.Database.WithData(context =>
        {
            var entity = context.InstitutionAccounts.Find(fixture.InstitutionAccountEntity.Id)
                         ?? throw FailException.ForFailure("InstitutionAccountEntity not found");

            callback(entity);
        });
    }
}
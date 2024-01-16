using App.Institution.Interface;
using App.Lib.Data.Entity;
using App.Institution.Service;
using App.Lib.Test;
using App.Lib.Test.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using App.Lib.ServiceBus;
using App.Lib.ServiceBus.Messages.Institution;
using Moq;
using NodaTime;

namespace App.Institution.Test.Service;

public class TransactionImportQueueServiceTest
{
    private readonly DatabasePool _databasePool;
    private readonly ILoggerProvider _loggerProvider;

    public TransactionImportQueueServiceTest(DatabasePool databasePool, ILoggerProvider loggerProvider)
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
        var service = fixture.Services.GetRequiredService<ITransactionImportQueueService>();

        // Assert
        service.Should().BeOfType<TransactionImportQueueService>();
    }


    [Fact]
    public async Task QueueRefreshJobsOnlyWhenNotAlreadyQueued()
    {
        // Arrange
        var fixture = new TransactionImportFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ITransactionImportQueueService>();
        var now = new LocalDateTime(2023, 12, 1, 10, 0).InUtc().ToInstant();
        fixture.Services.WithMock<IClock>(mock =>
        {
            mock.Setup(m => m.GetCurrentInstant()).Returns(now);
        });

        // Act
        var result = await service.QueueAccountAsync(fixture.InstitutionAccountEntity.Id);
        await service.QueueAccountAsync(fixture.InstitutionAccountEntity.Id);

        // Assert
        fixture.Services.WithMock<IServiceBus>(mock =>
        {
            mock.Verify(
                m => m.Send(
                    It.Is<TransactionImportJob>(job => job.InstitutionConnectionAccountId == fixture.InstitutionAccountEntity.Id),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        });
    }

    [Fact]
    public async Task QueueRefreshJobsMaxOncePer12Hours()
    {
        // Arrange
        var fixture = new TransactionImportFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<ITransactionImportQueueService>();
        var now = new LocalDateTime(2023, 12, 1, 10, 0).InUtc().ToInstant();
        fixture.Services.WithMock<IClock>(mock =>
        {
            mock.Setup(m => m.GetCurrentInstant()).Returns(now);
        });

        fixture.Database.SeedData(context =>
        {
            context.InstitutionAccounts.Add(new InstitutionAccountEntity()
            {
                Id = new Guid("1cb4f693-fd2b-46f0-8559-842dbd3090a4"),
                InstitutionConnectionId = fixture.InstitutionConnectionEntity.Id,
                ExternalId = "e0e3febd-7e7f-46c3-9376-04d5d4343ced",
                Iban = "IBAN-1",
                LastImportRequested = now
            });
            context.InstitutionAccounts.Add(new InstitutionAccountEntity()
            {
                Id = new Guid("31ba4686-d399-46ee-a2c7-f81644b5d61e"),
                InstitutionConnectionId = fixture.InstitutionConnectionEntity.Id,
                ExternalId = "d586a811-d757-4f65-aba6-22576b81abce",
                Iban = "IBAN-2",
                LastImportRequested = now.Minus(Duration.FromHours(13)),
            });
        });

        // Act
        await service.QueueAllAccountsAsync();
        await service.QueueAllAccountsAsync();

        // Assert
        fixture.Services.WithMock<IServiceBus>(mock =>
        {
            mock.Verify(
                m => m.Send(
                    It.Is<TransactionImportJob>(j =>
                        j.InstitutionConnectionAccountId == new Guid("31ba4686-d399-46ee-a2c7-f81644b5d61e")),
                    It.IsAny<CancellationToken>()),
                Times.Once());

            mock.Verify(
                m => m.Send(
                    It.IsAny<TransactionImportJob>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        });
    }
}
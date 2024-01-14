using App.Lib.Data.Entity;
using App.Institution.Exception;
using App.Institution.Service;
using App.Lib.ServiceBus;
using App.Lib.ServiceBus.Messages.Institution;
using App.Lib.Test;
using App.Lib.Test.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using VMelnalksnis.NordigenDotNet.Accounts;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Institution.Test.Service;

public class InstitutionConnectionRefreshServiceTest
{
    private readonly DatabasePool _databasePool;
    private readonly ILoggerProvider _loggerProvider;

    public InstitutionConnectionRefreshServiceTest(DatabasePool databasePool, ILoggerProvider loggerProvider)
    {
        _databasePool = databasePool;
        _loggerProvider = loggerProvider;
    }

    [Fact]
    public void Implementation()
    {
        // Arrange
        var fixture = new InstitutionConnectionRefreshFixture(_databasePool, _loggerProvider);

        // Act
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionRefreshService>();

        // Assert
        service.Should().BeOfType<InstitutionConnectionRefreshService>();
    }

    [Fact]
    public async Task ByExternalId_MissingConnection()
    {
        // Arrange
        var fixture = new InstitutionConnectionRefreshFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionRefreshService>();

        // Act
        var act = async () => await service.Refresh("SomeExternalIdMissing");

        // Assert
        var result = await Assert.ThrowsAsync<InstitutionConnectionNotFoundException>(act);
        result.Data["ExternalConnectionId"].Should().Be("SomeExternalIdMissing");

        fixture.Services.WithMock<IAccountClient>(mock =>
        {
            mock.Verify(
                m => m.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never());
        });
    }

    [Fact]
    public async Task ByExternalId_Success()
    {
        // Arrange
        var fixture = new InstitutionConnectionRefreshFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionRefreshService>();

        // Act
        var result = await service.Refresh(fixture.InstitutionConnectionEntity.ExternalId);
        result.Accounts.Should().HaveCount(2);

        // Assert
        fixture.Database.WithData(context =>
        {
            context.InstitutionAccounts
                .Where(a => a.InstitutionConnectionId == fixture.InstitutionConnectionEntity.Id)
                .ToList()
                .Should()
                .HaveCount(2);
        });
    }

    [Fact]
    public async Task ById_MissingConnection()
    {
        // Arrange
        var fixture = new InstitutionConnectionRefreshFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionRefreshService>();

        // Act
        var id = new Guid("59a35c45-6e8d-4dc7-bacc-f151f94da93d");
        var act = async () => await service.Refresh(id);

        // Assert
        var result = await Assert.ThrowsAsync<InstitutionConnectionNotFoundException>(act);
        result.Data["InstitutionConnectionId"].Should().Be(id);

        fixture.Services.WithMock<IAccountClient>(mock =>
        {
            mock.Verify(
                m => m.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never());
        });
    }

    [Fact]
    public async Task ById_Success()
    {
        // Arrange
        var fixture = new InstitutionConnectionRefreshFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionRefreshService>();

        // Act
        var result = await service.Refresh(fixture.InstitutionConnectionEntity.Id);
        result.Accounts
            .Should()
            .BeEquivalentTo(new List<InstitutionAccountEntity>()
            {
                new () { Iban = "IBAN-1" },
                new () { Iban = "IBAN-2" }
            },
            options => options.Including(a => a.Iban));

        // Assert
        fixture.Database.WithData(context =>
        {
            context.InstitutionAccounts
                .Where(a => a.InstitutionConnectionId == fixture.InstitutionConnectionEntity.Id)
                .ToList()
                .Should()
                .BeEquivalentTo(new List<InstitutionAccountEntity>()
                    {
                        new() { Iban = "IBAN-1", ImportStatus = ImportStatus.Queued },
                        new() { Iban = "IBAN-2", ImportStatus = ImportStatus.Queued }
                    },
                    options => options.Including(a => a.Iban));
        });
    }

    [Fact]
    public async Task ById_OnlyWithinOrganisation()
    {
        // Arrange
        var fixture = new InstitutionConnectionRefreshFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionRefreshService>();

        // Act
        var act = async () => await service
            .Refresh(fixture.OrganisationMissmatchInstitutionConnectionEntity.Id);

        // Assert
        await Assert.ThrowsAsync<InstitutionConnectionNotFoundException>(act);
    }

    [Fact]
    public async Task ByExternalId_OnlyWithinOrganisation()
    {
        // Arrange
        var fixture = new InstitutionConnectionRefreshFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionRefreshService>();

        // Act
        var act = async () => await service
            .Refresh(fixture.OrganisationMissmatchInstitutionConnectionEntity.ExternalId);

        // Assert
        await Assert.ThrowsAsync<InstitutionConnectionNotFoundException>(act);
    }

    [Fact]
    public async Task QueueRefreshJobsOnlyWhenNotAlreadyQueued()
    {
        // Arrange
        var fixture = new InstitutionConnectionRefreshFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionRefreshService>();
        var now = new LocalDateTime(2023, 12, 1, 10, 0).InUtc().ToInstant();
        fixture.Services.WithMock<IClock>(mock =>
        {
            mock.Setup(m => m.GetCurrentInstant()).Returns(now);
        });

        // Act
        var result = await service.Refresh(fixture.InstitutionConnectionEntity.ExternalId);
        await service.Refresh(fixture.InstitutionConnectionEntity.ExternalId);

        // Assert
        result.Accounts.Should().HaveCount(2);
        fixture.Services.WithMock<IServiceBus>(mock =>
        {
            foreach (var account in result.Accounts)
            {
                mock.Verify(
                    m => m.Send(
                        It.Is<TransactionImportJob>(job => job.InstitutionConnectionAccountId == account.Id),
                        It.IsAny<CancellationToken>()),
                    Times.Exactly(1));
            }
        });
    }

    [Fact]
    public async Task QueueRefreshJobsMaxOncePer12Hours()
    {
        // Arrange
        var fixture = new InstitutionConnectionRefreshFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionRefreshService>();
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
        await service.Refresh(fixture.InstitutionConnectionEntity.ExternalId);
        await service.Refresh(fixture.InstitutionConnectionEntity.ExternalId);

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

    [Fact]
    public async Task LimitRefreshAccountsOnlyWhenNoAccountsAreSet()
    {
        var fixture = new InstitutionConnectionRefreshFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionRefreshService>();
        fixture.Database.SeedData(context =>
        {
            context.InstitutionAccounts.Add(new InstitutionAccountEntity()
            {
                InstitutionConnectionId = fixture.InstitutionConnectionEntity.Id,
                ExternalId = "a1fd81b4-3ef1-4036-8899-d337314a1638",
                Iban = "IBAN-1"
            });
        });

        // Act
        await service.Refresh(fixture.InstitutionConnectionEntity.ExternalId);
        var result = await service.Refresh(fixture.InstitutionConnectionEntity.ExternalId);

        // Assert
        result.Accounts.Should().HaveCount(1);
        fixture.Services.WithMock<IRequisitionClient>(mock =>
        {
            mock.Verify(
                m => m.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never());
        });
    }
}
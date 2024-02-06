using App.Lib.Data.Entity;
using App.Institution.Interface;
using App.Institution.Service;
using App.Lib.Test;
using App.Lib.Test.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using VMelnalksnis.NordigenDotNet.Accounts;
using VMelnalksnis.NordigenDotNet.Requisitions;
using App.Lib.Data.Exception;

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
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(act);
        result.Data["Id"].Should().Be("SomeExternalIdMissing");

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
        var result = await Assert.ThrowsAsync<EntityNotFoundException>(act);
        result.Data["Id"].Should().Be(id.ToString());

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
        await Assert.ThrowsAsync<EntityNotFoundException>(act);
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
        await Assert.ThrowsAsync<EntityNotFoundException>(act);
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
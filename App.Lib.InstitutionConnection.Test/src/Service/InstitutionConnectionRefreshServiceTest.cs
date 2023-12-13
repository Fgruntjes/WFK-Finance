using App.Lib.Data.Entity;
using App.Lib.InstitutionConnection.Exception;
using App.Lib.InstitutionConnection.Service;
using App.Lib.Test;
using App.Lib.Test.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using VMelnalksnis.NordigenDotNet.Accounts;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Lib.InstitutionConnection.Test.Service;

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
            context.InstitutionConnectionAccounts
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
            context.InstitutionConnectionAccounts
                .Where(a => a.InstitutionConnectionId == fixture.InstitutionConnectionEntity.Id)
                .ToList()
                .Should()
                .BeEquivalentTo(new List<InstitutionAccountEntity>()
                    {
                        new() { Iban = "IBAN-1" },
                        new() { Iban = "IBAN-2" }
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
    public async Task ById_RemoveOldAccounts()
    {
        // Arrange
        var fixture = new InstitutionConnectionRefreshFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionRefreshService>();
        fixture.Database.SeedData(context =>
        {
            context.InstitutionConnectionAccounts.Add(new InstitutionAccountEntity()
            {
                InstitutionConnectionId = fixture.InstitutionConnectionEntity.Id,
                ExternalId = "a1fd81b4-3ef1-4036-8899-d337314a1638",
                Iban = "IBAN-1"
            });
            context.InstitutionConnectionAccounts.Add(new InstitutionAccountEntity()
            {
                InstitutionConnectionId = fixture.InstitutionConnectionEntity.Id,
                ExternalId = "582a2fcd-b842-45e3-b1b3-ab2a54e42127",
                Iban = "IBAN-2"
            });
        });
        fixture.Services.WithMock<IRequisitionClient>(mock =>
        {
            var externalGuid = new Guid(fixture.InstitutionConnectionEntity.ExternalId);
            var nordigenRequisitionResult = new Requisition
            {
                Link = new Uri("https://www.example.com/connect-url/nordigen-requisition"),
                Id = externalGuid,
                Accounts = new List<Guid> { new("582a2fcd-b842-45e3-b1b3-ab2a54e42127") }
            };

            mock.Reset();
            mock.Setup(r => r.Get(externalGuid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(nordigenRequisitionResult);
        });

        // Act
        await service.Refresh(fixture.InstitutionConnectionEntity.Id);

        // Assert
        fixture.Database.WithData(context =>
        {
            context.InstitutionConnectionAccounts
                .Where(a => a.InstitutionConnectionId == fixture.InstitutionConnectionEntity.Id)
                .ToList()
                .Should()
                .BeEquivalentTo(new List<InstitutionAccountEntity>
                {
                    new () { ExternalId = "582a2fcd-b842-45e3-b1b3-ab2a54e42127" }
                }, options => options.Including(a => a.ExternalId));
        });
    }

    [Fact]
    public async Task ById_AddNewAccounts()
    {
        // Arrange
        var fixture = new InstitutionConnectionRefreshFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionRefreshService>();
        var newAccountId = new Guid("582a2fcd-b842-45e3-b1b3-ab2a54e42127");
        fixture.Database.SeedData(context =>
        {
            context.InstitutionConnectionAccounts.Add(new InstitutionAccountEntity()
            {
                InstitutionConnectionId = fixture.InstitutionConnectionEntity.Id,
                ExternalId = "a1fd81b4-3ef1-4036-8899-d337314a1638",
                Iban = "IBAN-1"
            });
        });
        fixture.Services.WithMock<IRequisitionClient>(mock =>
        {
            var externalGuid = new Guid(fixture.InstitutionConnectionEntity.ExternalId);
            var nordigenRequisitionResult = new Requisition
            {
                Link = new Uri("https://www.example.com/connect-url/nordigen-requisition"),
                Id = externalGuid,
                Accounts = new List<Guid>
                {
                    new ("a1fd81b4-3ef1-4036-8899-d337314a1638"),
                    newAccountId
                }
            };

            mock.Reset();
            mock.Setup(r => r.Get(externalGuid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(nordigenRequisitionResult);
        });
        fixture.Services.WithMock<IAccountClient>(mock =>
        {
            mock.Setup(a => a.Get(newAccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Account()
                {
                    Id = newAccountId,
                    Iban = "IBAN-2",
                });
        });

        // Act
        await service.Refresh(fixture.InstitutionConnectionEntity.Id);

        // Assert
        fixture.Database.WithData(context =>
        {
            context.InstitutionConnectionAccounts
                .Where(a => a.InstitutionConnectionId == fixture.InstitutionConnectionEntity.Id)
                .ToList()
                .Should()
                .BeEquivalentTo(
                    new List<InstitutionAccountEntity>
                    {
                        new () { ExternalId = "a1fd81b4-3ef1-4036-8899-d337314a1638", Iban = "IBAN-1"},
                        new () { ExternalId = "582a2fcd-b842-45e3-b1b3-ab2a54e42127", Iban = "IBAN-2" }
                    },
                    options => options
                        .Including(a => a.ExternalId)
                        .Including(a => a.Iban));
        });
    }

    [Fact]
    public async Task ById_DoNothingWhenEqual()
    {
        // Arrange
        var fixture = new InstitutionConnectionRefreshFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionRefreshService>();
        fixture.Database.SeedData(context =>
        {
            context.InstitutionConnectionAccounts.Add(new InstitutionAccountEntity()
            {
                InstitutionConnectionId = fixture.InstitutionConnectionEntity.Id,
                ExternalId = "a1fd81b4-3ef1-4036-8899-d337314a1638",
                Iban = "IBAN-1"
            });
            context.InstitutionConnectionAccounts.Add(new InstitutionAccountEntity()
            {
                InstitutionConnectionId = fixture.InstitutionConnectionEntity.Id,
                ExternalId = "582a2fcd-b842-45e3-b1b3-ab2a54e42127",
                Iban = "IBAN-2"
            });
        });
        fixture.Services.WithMock<IRequisitionClient>(mock =>
        {
            var externalGuid = new Guid(fixture.InstitutionConnectionEntity.ExternalId);
            var nordigenRequisitionResult = new Requisition
            {
                Link = new Uri("https://www.example.com/connect-url/nordigen-requisition"),
                Id = externalGuid,
                Accounts = new List<Guid>
                {
                    new ("a1fd81b4-3ef1-4036-8899-d337314a1638"),
                    new ("582a2fcd-b842-45e3-b1b3-ab2a54e42127"),
                }
            };

            mock.Reset();
            mock.Setup(r => r.Get(externalGuid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(nordigenRequisitionResult);
        });

        // Act
        await service.Refresh(fixture.InstitutionConnectionEntity.Id);

        // Assert
        fixture.Database.WithData(context =>
        {
            context.InstitutionConnectionAccounts
                .Where(a => a.InstitutionConnectionId == fixture.InstitutionConnectionEntity.Id)
                .ToList()
                .Should()
                .HaveCount(2);
        });
        fixture.Services.WithMock<IAccountClient>(mock =>
        {
            mock.Verify(m => m.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        });
    }
}
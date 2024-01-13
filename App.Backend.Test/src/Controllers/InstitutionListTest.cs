using App.Backend.Controllers;
using App.Backend.Dto;
using App.Lib.Data.Entity;
using App.Lib.InstitutionConnection.Service;
using App.Lib.Test;
using App.Lib.Test.Database;
using Gridify;
using Microsoft.Extensions.Logging;
using Moq;

namespace App.Backend.Test.Controllers;

public class InstitutionListTest
{
    private readonly DatabasePool _databasePool;
    private readonly ILoggerProvider _loggerProvider;

    public InstitutionListTest(DatabasePool databasePool, ILoggerProvider loggerProvider)
    {
        _databasePool = databasePool;
        _loggerProvider = loggerProvider;
    }

    [Fact]
    public async Task FilterCountry()
    {
        // Arrange
        var fixture = new AppFixture(_databasePool, _loggerProvider);
        fixture.Services.WithMock<IInstitutionSearchService>(mock =>
        {
            mock.Setup(s => s.Search(
                "NL",
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InstitutionEntity>()
                {
                    new ()
                    {
                        Name = "MyFakeName - NL1",
                        ExternalId = "SomeExternalId-NL1",
                        CountryIso2 = "NL"
                    },
                    new ()
                    {
                        Name = "MyFakeName - NL2",
                        ExternalId = "SomeExternalId-NL2",
                        CountryIso2 = "NL"
                    }
                });
        });

        // Act
        var result = await fixture.Client.GetListWithAuthAsync<Institution>(
            InstitutionListController.RouteBase,
            query: new GridifyQuery()
            {
                Filter = "countryIso2 = NL"
            });

        // Assert
        result.Should().BeEquivalentTo(new List<Institution>()
        {
            new ()
            {
                Name = "MyFakeName - NL1",
                ExternalId = "SomeExternalId-NL1",
                CountryIso2 = "NL"
            },
            new ()
            {
                Name = "MyFakeName - NL2",
                ExternalId = "SomeExternalId-NL2",
                CountryIso2 = "NL"
            }
        },
        options => options.Excluding(e => e.Id));
    }

    [Fact]
    public async Task SearchNotFound()
    {
        // Act
        var fixture = new AppFixture(_databasePool, _loggerProvider);
        var result = await fixture.Client.GetListWithAuthAsync<Institution>(
            InstitutionListController.RouteBase,
            query: new GridifyQuery()
            {
                Filter = "countryIso2 = BE"
            });

        // Assert
        result.Should().HaveCount(0);
    }

    [Fact]
    public async Task FilterById()
    {
        // Arrange
        var fixture = new AppFixture(_databasePool, _loggerProvider);
        fixture.Database.SeedData(context =>
        {
            context.Institutions.Add(new InstitutionEntity()
            {
                Id = new Guid("2fdc768e-e916-47ca-9f09-90f939547fa8"),
                Name = "MyFakeName - NL1",
                ExternalId = "SomeExternalId-NL1",
                CountryIso2 = "NL"
            });
            context.Institutions.Add(new InstitutionEntity()
            {
                Id = new Guid("2fdc768e-e916-47ca-9f09-90f939547fa9"),
                Name = "MyFakeName - NL2",
                ExternalId = "SomeExternalId-NL2",
                CountryIso2 = "NL"
            });
            context.Institutions.Add(new InstitutionEntity()
            {
                Id = new Guid("2fdc768e-e916-47ca-9f09-90f939547fa0"),
                Name = "MyFakeName - NL3",
                ExternalId = "SomeExternalId-NL3",
                CountryIso2 = "NL"
            });
        });

        // Act
        var result = await fixture.Client.GetListWithAuthAsync<Institution>(
            InstitutionListController.RouteBase,
            query: new GridifyQuery()
            {
                Filter = "id = 2fdc768e-e916-47ca-9f09-90f939547fa8 | id = 2fdc768e-e916-47ca-9f09-90f939547fa0"
            });

        // Assert
        result.Should().BeEquivalentTo(new List<Institution>()
        {
            new ()
            {
                Name = "MyFakeName - NL1",
                ExternalId = "SomeExternalId-NL1",
                CountryIso2 = "NL"
            },
            new ()
            {
                Name = "MyFakeName - NL3",
                ExternalId = "SomeExternalId-NL3",
                CountryIso2 = "NL"
            }
        },
            options => options.Excluding(e => e.Id));
    }
}
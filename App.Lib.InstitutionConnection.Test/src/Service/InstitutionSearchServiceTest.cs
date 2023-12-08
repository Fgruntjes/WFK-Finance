using App.Lib.Data.Entity;
using App.Lib.InstitutionConnection.Service;
using App.Lib.Test;
using App.Lib.Test.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Institutions;

namespace App.Lib.InstitutionConnection.Test.Service;

public class InstitutionSearchServiceTest
{
    private readonly DatabasePool _databasePool;
    private readonly ILoggerProvider _loggerProvider;

    public InstitutionSearchServiceTest(DatabasePool databasePool, ILoggerProvider loggerProvider)
    {
        _databasePool = databasePool;
        _loggerProvider = loggerProvider;
    }

    [Fact]
    public void Implementation()
    {
        // Arrange
        var fixture = new AppFixture(_databasePool, _loggerProvider);

        // Act
        var service = fixture.Services.GetRequiredService<IInstitutionSearchService>();

        // Assert
        service.Should().BeOfType<InstitutionSearchService>();
    }

    [Fact]
    public async Task CallRefreshWhenMissing()
    {
        // Arrange
        var fixture = new AppFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionSearchService>();

        var institutionsMock = new Mock<IInstitutionClient>();
        institutionsMock
            .Setup(i => i.GetByCountry("GB", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Institution>()
            {
                new (){
                    Id = "SomeExternalId-GB-1",
                    Name = "MyFakeName-GB-1"
                },
                new (){
                    Id = "SomeExternalId-GB-2",
                    Name = "MyFakeName-GB-2"
                }
            });

        fixture.Services.WithMock<INordigenClient>(mock =>
        {
            mock
                .SetupGet(c => c.Institutions)
                .Returns(institutionsMock.Object);
        });

        // Act
        var result = await service.Search("GB");

        // Arrange
        result.Should()
            .BeEquivalentTo(
                new List<InstitutionEntity>
                {
                    new () { ExternalId = "SomeExternalId-GB-1", Name = "MyFakeName-GB-1" },
                    new () { ExternalId = "SomeExternalId-GB-2", Name = "MyFakeName-GB-2" }
                },
                options => options
                    .Including(a => a.ExternalId)
                    .Including(a => a.Name));
    }

    [Fact]
    public async Task Search()
    {
        // Arrange
        var fixture = new AppFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionSearchService>();

        var institutionNldLinked = new InstitutionEntity()
        {
            Name = "MyFakeName-NL",
            ExternalId = "SomeExternalId-NL",
            CountryIso2 = "NL"
        };
        var institutionUsaLinked = new InstitutionEntity()
        {
            Name = "MyFakeName-US",
            ExternalId = "SomeExternalId-US",
            CountryIso2 = "US"
        };
        fixture.Database.SeedData(context =>
        {
            context.Institutions.Add(institutionNldLinked);
            context.Institutions.Add(institutionUsaLinked);
        });

        // Act
        var result = await service.Search("NL");

        // Assert
        result.Should()
            .BeEquivalentTo(new List<InstitutionEntity>
            {
                new () { Name = "MyFakeName-NL" }
            },
            options => options.Including(a => a.Name));
    }
}
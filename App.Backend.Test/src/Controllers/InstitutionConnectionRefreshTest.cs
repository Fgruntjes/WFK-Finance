using Microsoft.EntityFrameworkCore;
using Xunit.Sdk;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionRefreshTest : IClassFixture<InstitutionConnectionRefreshFixture>
{
    private readonly InstitutionConnectionRefreshFixture _fixture;

    public InstitutionConnectionRefreshTest(InstitutionConnectionRefreshFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ByExternalId_Success()
    {
        // Act
        var result = await _fixture.Server.ExecuteQuery(new
        {
            _fixture.InstitutionConnectionEntity.ExternalId
        });

        // Assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task ByExternalId_MissingConnection()
    {
        // Act
        var result = await _fixture.Server.ExecuteQuery(new
        {
            ExternalId = "SomeExternalIdMissing"
        });

        // Assert
        result.MatchSnapshot(m => m.IgnoreField("errors[0].extensions.timestamp"));
    }

    [Fact]
    public async Task ById_Success()
    {
        // Act
        var result = await _fixture.Server.ExecuteQuery(new
        {
            _fixture.InstitutionConnectionEntity.Id
        });

        // Assert response
        result.MatchSnapshot();

        // Assert database
        _fixture.WithData(context =>
        {
            context.InstitutionConnections
                .Include(e => e.Accounts)
                .First(e => e.Id == _fixture.InstitutionConnectionEntity.Id)
                .Accounts.Should().HaveCount(2);
        });
    }

    [Fact]
    public async Task ById_MissingConnection()
    {
        // Act
        var result = await _fixture.Server.ExecuteQuery(new
        {
            Id = new Guid("59a35c45-6e8d-4dc7-bacc-f151f94da93d")
        });

        // Assert
        result.MatchSnapshot(m => m.IgnoreField("errors[0].extensions.timestamp"));
    }

    [Fact]
    public async Task OnlyWithinOrganisation()
    {
        FailException.ForFailure("Not implemented");
    }
}
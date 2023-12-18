using App.Lib.InstitutionConnection.Exception;
using App.Lib.InstitutionConnection.Service;
using App.Lib.Test;
using Moq;

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
        // Arrange
        _fixture.Services.WithMock<IInstitutionConnectionRefreshService>(mock =>
        {
            mock.Setup(e => e.Refresh(
                    _fixture.InstitutionConnectionEntity.ExternalId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_fixture.InstitutionConnectionEntity);
        });

        // Act
        var result = await _fixture.Client.ExecuteQuery(new
        {
            _fixture.InstitutionConnectionEntity.ExternalId
        });

        // Assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task ByExternalId_MissingConnection()
    {
        // Arrange
        _fixture.Services.WithMock<IInstitutionConnectionRefreshService>(mock =>
        {
            mock.Setup(e => e.Refresh(
                    "SomeExternalIdMissing",
                    It.IsAny<CancellationToken>()))
                .Callback((string externalConnectionId, CancellationToken _)
                    => throw new InstitutionConnectionNotFoundException(externalConnectionId));
        });

        // Act
        var result = await _fixture.Client.ExecuteQuery(new
        {
            ExternalId = "SomeExternalIdMissing"
        });

        // Assert
        result.MatchSnapshot(m => m
            .IgnoreField("errors[0].extensions.timestamp")
            .IgnoreField("errors[0].extensions.exception"));
    }

    [Fact]
    public async Task ById_Success()
    {
        // Arrange
        _fixture.Services.WithMock<IInstitutionConnectionRefreshService>(mock =>
        {
            mock.Setup(e => e.Refresh(
                    _fixture.InstitutionConnectionEntity.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_fixture.InstitutionConnectionEntity);
        });

        // Act
        var result = await _fixture.Client.ExecuteQuery(new
        {
            _fixture.InstitutionConnectionEntity.Id
        });

        // Assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task ById_MissingConnection()
    {
        // Arrange
        var id = new Guid("59a35c45-6e8d-4dc7-bacc-f151f94da93d");
        _fixture.Services.WithMock<IInstitutionConnectionRefreshService>(mock =>
        {
            mock.Setup(e => e.Refresh(
                    id,
                    It.IsAny<CancellationToken>()))
                .Callback((Guid institutionConnectionId, CancellationToken _)
                    => throw new InstitutionConnectionNotFoundException(institutionConnectionId));
        });

        // Act
        var result = await _fixture.Client.ExecuteQuery(new
        {
            Id = id
        });

        // Assert
        result.MatchSnapshot(m => m
            .IgnoreField("errors[0].extensions.timestamp")
            .IgnoreField("errors[0].extensions.exception"));
    }
}
using System.Net;
using System.Net.Http.Json;
using App.Backend.Controllers;
using App.Backend.Dto;
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
        var externalId = _fixture.InstitutionConnectionEntity.ExternalId;
        var response = await _fixture.Client.SendWithAuthAsync(
            new HttpRequestMessage(HttpMethod.Put, $"{InstitutionConnectionListController.RouteBase}/refresh/external/{externalId}"));
        var body = await response.Content.ReadFromJsonAsync<InstitutionConnection>();

        // Assert
        body.Should().BeEquivalentTo(new InstitutionConnection
        {
            Id = _fixture.InstitutionConnectionEntity.Id,
            InstitutionId = _fixture.InstitutionConnectionEntity.InstitutionId,
            ExternalId = _fixture.InstitutionConnectionEntity.ExternalId,
            ConnectUrl = _fixture.InstitutionConnectionEntity.ConnectUrl,
            Accounts = new List<InstitutionConnectionAccount>(),
        });
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
        var response = await _fixture.Client.SendWithAuthAsync(
            new HttpRequestMessage(HttpMethod.Put, $"{InstitutionConnectionListController.RouteBase}/refresh/external/SomeExternalIdMissing"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _fixture.Services.WithMock<IInstitutionConnectionRefreshService>(mock =>
        {
            mock.Verify(e => e.Refresh(
                "SomeExternalIdMissing",
                It.IsAny<CancellationToken>()), Times.Once);
        });
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
        var id = _fixture.InstitutionConnectionEntity.Id;
        var response = await _fixture.Client.SendWithAuthAsync(
            new HttpRequestMessage(HttpMethod.Put, $"{InstitutionConnectionListController.RouteBase}/refresh/id/{id}"));
        var body = await response.Content.ReadFromJsonAsync<InstitutionConnection>();

        // Assert
        body.Should().BeEquivalentTo(new InstitutionConnection
        {
            Id = _fixture.InstitutionConnectionEntity.Id,
            InstitutionId = _fixture.InstitutionConnectionEntity.InstitutionId,
            ExternalId = _fixture.InstitutionConnectionEntity.ExternalId,
            ConnectUrl = _fixture.InstitutionConnectionEntity.ConnectUrl,
            Accounts = new List<InstitutionConnectionAccount>(),
        });
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
        var response = await _fixture.Client.SendWithAuthAsync(
            new HttpRequestMessage(HttpMethod.Put, $"{InstitutionConnectionListController.RouteBase}/refresh/id/{id}"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _fixture.Services.WithMock<IInstitutionConnectionRefreshService>(mock =>
        {
            mock.Verify(e => e.Refresh(
                id,
                It.IsAny<CancellationToken>()), Times.Once);
        });
    }
}
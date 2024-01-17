using System.Net;
using App.Backend.Controllers;
using App.Institution.Interface;
using App.Lib.Test;
using Moq;

namespace App.Backend.Test.Controllers;

public class InstitutionTransactionImportCronTest : IClassFixture<InstitutionAccountTransactionListFixture>
{
    private readonly InstitutionAccountTransactionListFixture _fixture;

    public InstitutionTransactionImportCronTest(InstitutionAccountTransactionListFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task InvalidCronToken()
    {
        // Act
        var response = await _fixture.Client.SendAsync(
            new HttpRequestMessage(HttpMethod.Post, $"{InstitutionTransactionImportCronController.RouteBase}?cronToken=invalid"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task QueueForEachAccount()
    {
        // Arrange
        _fixture.Services.WithMock<ITransactionImportQueueService>(mock =>
        {
            mock.Setup(e => e.QueueAllAccountsAsync(It.IsAny<CancellationToken>()));
        });

        // Act
        var response = await _fixture.Client.SendAsync(
            new HttpRequestMessage(HttpMethod.Post, $"{InstitutionTransactionImportCronController.RouteBase}?cronToken=somerandomtoken"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        _fixture.Services.WithMock<ITransactionImportQueueService>(mock =>
        {
            mock.Verify(e => e.QueueAllAccountsAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }
}

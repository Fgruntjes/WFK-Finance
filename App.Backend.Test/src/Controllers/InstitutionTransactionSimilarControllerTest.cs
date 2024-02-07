using App.Backend.Controllers;
using App.Lib.Data.Entity;
using App.TransactionCategory.Interface;
using Moq;
using App.Lib.Test;
using App.Lib.Data.Exception;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using NodaTime;
using App.Backend.Dto;

namespace App.Backend.Test.Controllers;

public class InstitutionTransactionSimilarControllerTest : IClassFixture<AppFixture>
{
    private readonly AppFixture _fixture;

    public InstitutionTransactionSimilarControllerTest(AppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Success()
    {
        // Arrange
        var id = new Guid("eeef2571-9d3c-47cc-a6b0-b15ed18f2229");
        var resultList = new List<InstitutionAccountTransactionEntity>
        {
            new()
            {
                Id = new Guid("eeef2571-9d3c-47cc-a6b0-b15ed18f2229"),
                CreatedAt = Instant.FromUnixTimeTicks(637717792000000000),
                Date = Instant.FromUnixTimeTicks(637717792000000000),
                Amount = 100,
                UnstructuredInformation = "Test",
            }
        };
        _fixture.Services.WithMock<ISimilarTransactionService>((mock) =>
        {
            mock.Setup(x => x.Find(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultList);
        });

        // Act
        var result = await _fixture.Client.GetWithAuthAsync<ICollection<InstitutionTransactionDto>>(
            $"{InstitutionTransactionListController.RouteBase}/{id}/similar");

        // Assert
        result.Should().BeEquivalentTo(new List<InstitutionTransactionDto>
        {
            new()
            {
                Id = new Guid("eeef2571-9d3c-47cc-a6b0-b15ed18f2229"),
                Date = Instant.FromUnixTimeTicks(637717792000000000).ToDateTimeUtc(),
                Amount = 100,
                UnstructuredInformation = "Test",
            }
        });
    }

    [Fact]
    public async Task NotFound()
    {
        // Arrange
        var id = new Guid("b36eaeda-4369-4d9c-81bb-b222c4404bd8");
        _fixture.Services.WithMock<ISimilarTransactionService>((mock) =>
        {
            mock.Setup(x => x.Find(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new EntityNotFoundException(id));
        });

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(new HttpRequestMessage(
            HttpMethod.Get,
            $"{InstitutionTransactionListController.RouteBase}/{id}/similar"));

        // Assert
        await response.AssertProblemDetails(HttpStatusCode.NotFound, new ProblemDetails()
        {
            Title = "An error occurred",
            Detail = "Id {Id} not found",
            Status = (int)HttpStatusCode.NotFound,
            Extensions =
            {
                { "Id", id }
            }
        });
    }
}
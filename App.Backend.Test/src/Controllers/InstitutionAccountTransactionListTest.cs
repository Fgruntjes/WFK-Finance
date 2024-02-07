using System.Net;
using App.Backend.Controllers;
using App.Backend.Dto;
using App.Lib.Data.Entity;
using Gridify;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace App.Backend.Test.Controllers;

public class InstitutionAccountTransactionListTest : IClassFixture<InstitutionAccountTransactionListFixture>
{
    private readonly InstitutionAccountTransactionListFixture _fixture;

    public InstitutionAccountTransactionListTest(InstitutionAccountTransactionListFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Success()
    {
        // Act
        var response = await _fixture.Client.GetListWithAuthAsync(
            InstitutionAccountTransactionListController.RouteBase.Replace("{id:guid}", _fixture.InstitutionAccountEntity.Id.ToString()));
        var body = await response.Content.ReadFromJsonAsync<ICollection<InstitutionTransactionDto>>();

        // Assert
        body.Should().BeEquivalentTo(new List<InstitutionTransactionDto>()
        {
            new() { Date = _fixture.Now.ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(1)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(2)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(3)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(4)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(5)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(6)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(7)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(8)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(9)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(10)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(11)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(12)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(13)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(14)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(15)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(16)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(17)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(18)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(19)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(20)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(21)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(22)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(23)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(24)).ToDateTimeUtc() },
        }, options => options
            .Including(e => e.Date));

        response.Content.Headers.GetValues("Content-Range").First().Should().Be("institutionaccounttransactions 0-25/30");
    }

    [Fact]
    public async Task WithSort()
    {
        // Act
        var body = await _fixture.Client.GetListWithAuthAsync<InstitutionTransactionDto>(
            InstitutionAccountTransactionListController.RouteBase.Replace("{id:guid}", _fixture.InstitutionAccountEntity.Id.ToString()),
            query: new GridifyQuery()
            {
                OrderBy = "date desc"
            });

        // Assert
        body.Should().BeEquivalentTo(new List<InstitutionTransactionDto>()
        {
            new() { Date = _fixture.Now.Plus(Duration.FromHours(29)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(28)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(27)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(26)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(25)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(24)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(23)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(22)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(21)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(20)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(19)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(18)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(17)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(16)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(15)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(14)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(13)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(12)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(11)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(10)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(9)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(8)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(7)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(6)).ToDateTimeUtc() },
            new() { Date = _fixture.Now.Plus(Duration.FromHours(5)).ToDateTimeUtc() },
        }, options => options
            .Including(e => e.Date));
    }

    [Fact]
    public async Task WithSort_InvalidProperty()
    {
        // Act
        var response = await _fixture.Client.GetListWithAuthAsync(
            InstitutionAccountTransactionListController.RouteBase.Replace("{id:guid}", _fixture.InstitutionAccountEntity.Id.ToString()),
            query: new GridifyQuery()
            {
                OrderBy = "InvalidProperty desc"
            });

        // Assert
        await response.AssertProblemDetails(HttpStatusCode.BadRequest, new ProblemDetails
        {
            Title = "An error occurred",
            Detail = "Property {Property} is not found",
            Status = (int)HttpStatusCode.BadRequest,
            Extensions =
            {
                { "Property", "InvalidProperty" },
            }
        });
    }

    [Fact]
    public async Task WithRange()
    {
        var response = await _fixture.Client.GetListWithAuthAsync(
            InstitutionAccountTransactionListController.RouteBase.Replace("{id:guid}", _fixture.InstitutionAccountEntity.Id.ToString()),
            query: new GridifyQuery()
            {
                Page = 2,
                PageSize = 1
            });
        var body = await response.Content.ReadFromJsonAsync<ICollection<InstitutionTransactionDto>>();

        body.Should().BeEquivalentTo(new List<InstitutionTransactionDto>()
        {
            new() { Date = _fixture.Now.Plus(Duration.FromHours(1)).ToDateTimeUtc() },
        }, options => options
            .Including(e => e.Date));

        response.Content.Headers.GetValues("Content-Range").First().Should().Be("institutionaccounttransactions 1-2/30");
    }

    [Fact]
    public async Task WithFilter()
    {
        // Act
        var response = await _fixture.Client.GetListWithAuthAsync(
            InstitutionAccountTransactionListController.RouteBase.Replace("{id:guid}", _fixture.InstitutionAccountEntity.Id.ToString()),
            query: new GridifyQuery()
            {
                Filter = "Date = 2023-12-01T10:00:00Z"
            });
        var body = await response.Content.ReadFromJsonAsync<ICollection<InstitutionTransactionDto>>();

        // Assert
        body.Should().BeEquivalentTo(new List<InstitutionTransactionDto>()
        {
            new() { Date = new DateTime(2023, 12, 1, 10, 0, 0) },
        }, options => options.Including(e => e.Date));

        response.Content.Headers.GetValues("Content-Range").First().Should().Be("institutionaccounttransactions 0-1/1");
    }

    [Fact]
    public async Task WithFilter_InvalidProperty()
    {
        // Act
        var response = await _fixture.Client.GetListWithAuthAsync(
            InstitutionAccountTransactionListController.RouteBase.Replace("{id:guid}", _fixture.InstitutionAccountEntity.Id.ToString()),
            query: new GridifyQuery()
            {
                Filter = "InvalidProperty = SomeValue"
            });

        // Assert
        await response.AssertProblemDetails(HttpStatusCode.BadRequest, new ProblemDetails
        {
            Title = "An error occurred",
            Detail = "Property {Property} is not found",
            Status = (int)HttpStatusCode.BadRequest,
            Extensions =
            {
                { "Property", "InvalidProperty" }
            }
        });
    }

    [Fact]
    public async Task UnknownAccount()
    {
        // Act
        var response = await _fixture.Client.GetListWithAuthAsync(
            InstitutionAccountTransactionListController.RouteBase.Replace("{id:guid}", "b19abe01-6fbe-485d-9ff6-c032c1cdca6a"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AccountWithinOrganisation()
    {
        // Arrange
        var connectionEntity = new InstitutionConnectionEntity()
        {
            ExternalId = "SomeExternalId-organisation-missmatch-0",
            ConnectUrl = new Uri($"https://www.example-organisation-missmatch-0.com/"),
            InstitutionId = _fixture.InstitutionEntity.Id,
            OrganisationId = _fixture.AltOrganisationId,
        };
        var accountEntity = new InstitutionAccountEntity()
        {
            ExternalId = "SomeExternalId-organisation-missmatch-0",
            Iban = "NL00BANK0123456789",
            InstitutionConnectionId = connectionEntity.Id,
        };
        _fixture.Database.SeedData(c =>
        {
            c.InstitutionConnections.Add(connectionEntity);
            c.InstitutionAccounts.Add(accountEntity);
        });

        // Act
        var response = await _fixture.Client.GetListWithAuthAsync(
            InstitutionAccountTransactionListController.RouteBase.Replace("{id:guid}", accountEntity.Id.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

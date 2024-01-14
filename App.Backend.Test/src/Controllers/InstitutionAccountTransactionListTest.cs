using System.Net;
using System.Net.Http.Json;
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
        var body = await response.Content.ReadFromJsonAsync<ICollection<InstitutionAccountTransactionDto>>();

        // Assert
        body.Should().BeEquivalentTo(new List<InstitutionAccountTransactionDto>()
        {
            new() { ExternalId = $"SomeExternalId-organisation-match-0", Date = _fixture.Now.ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-1", Date = _fixture.Now.Plus(Duration.FromHours(1)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-2", Date = _fixture.Now.Plus(Duration.FromHours(2)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-3", Date = _fixture.Now.Plus(Duration.FromHours(3)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-4", Date = _fixture.Now.Plus(Duration.FromHours(4)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-5", Date = _fixture.Now.Plus(Duration.FromHours(5)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-6", Date = _fixture.Now.Plus(Duration.FromHours(6)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-7", Date = _fixture.Now.Plus(Duration.FromHours(7)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-8", Date = _fixture.Now.Plus(Duration.FromHours(8)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-9", Date = _fixture.Now.Plus(Duration.FromHours(9)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-10", Date = _fixture.Now.Plus(Duration.FromHours(10)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-11", Date = _fixture.Now.Plus(Duration.FromHours(11)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-12", Date = _fixture.Now.Plus(Duration.FromHours(12)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-13", Date = _fixture.Now.Plus(Duration.FromHours(13)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-14", Date = _fixture.Now.Plus(Duration.FromHours(14)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-15", Date = _fixture.Now.Plus(Duration.FromHours(15)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-16", Date = _fixture.Now.Plus(Duration.FromHours(16)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-17", Date = _fixture.Now.Plus(Duration.FromHours(17)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-18", Date = _fixture.Now.Plus(Duration.FromHours(18)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-19", Date = _fixture.Now.Plus(Duration.FromHours(19)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-20", Date = _fixture.Now.Plus(Duration.FromHours(20)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-21", Date = _fixture.Now.Plus(Duration.FromHours(21)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-22", Date = _fixture.Now.Plus(Duration.FromHours(22)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-23", Date = _fixture.Now.Plus(Duration.FromHours(23)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-24", Date = _fixture.Now.Plus(Duration.FromHours(24)).ToDateTimeUtc() },
        }, options => options
            .Including(e => e.ExternalId)
            .Including(e => e.Date));

        response.Content.Headers.GetValues("Content-Range").First().Should().Be("institutionaccounttransactions 0-25/30");
    }

    [Fact]
    public async Task WithSort()
    {
        // Act
        var body = await _fixture.Client.GetListWithAuthAsync<InstitutionAccountTransactionDto>(
            InstitutionAccountTransactionListController.RouteBase.Replace("{id:guid}", _fixture.InstitutionAccountEntity.Id.ToString()),
            query: new GridifyQuery()
            {
                OrderBy = "date desc"
            });

        // Assert
        body.Should().BeEquivalentTo(new List<InstitutionAccountTransactionDto>()
        {
            new() { ExternalId = $"SomeExternalId-organisation-match-29", Date = _fixture.Now.Plus(Duration.FromHours(29)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-28", Date = _fixture.Now.Plus(Duration.FromHours(28)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-27", Date = _fixture.Now.Plus(Duration.FromHours(27)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-26", Date = _fixture.Now.Plus(Duration.FromHours(26)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-25", Date = _fixture.Now.Plus(Duration.FromHours(25)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-24", Date = _fixture.Now.Plus(Duration.FromHours(24)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-23", Date = _fixture.Now.Plus(Duration.FromHours(23)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-22", Date = _fixture.Now.Plus(Duration.FromHours(22)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-21", Date = _fixture.Now.Plus(Duration.FromHours(21)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-20", Date = _fixture.Now.Plus(Duration.FromHours(20)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-19", Date = _fixture.Now.Plus(Duration.FromHours(19)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-18", Date = _fixture.Now.Plus(Duration.FromHours(18)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-17", Date = _fixture.Now.Plus(Duration.FromHours(17)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-16", Date = _fixture.Now.Plus(Duration.FromHours(16)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-15", Date = _fixture.Now.Plus(Duration.FromHours(15)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-14", Date = _fixture.Now.Plus(Duration.FromHours(14)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-13", Date = _fixture.Now.Plus(Duration.FromHours(13)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-12", Date = _fixture.Now.Plus(Duration.FromHours(12)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-11", Date = _fixture.Now.Plus(Duration.FromHours(11)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-10", Date = _fixture.Now.Plus(Duration.FromHours(10)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-9", Date = _fixture.Now.Plus(Duration.FromHours(9)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-8", Date = _fixture.Now.Plus(Duration.FromHours(8)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-7", Date = _fixture.Now.Plus(Duration.FromHours(7)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-6", Date = _fixture.Now.Plus(Duration.FromHours(6)).ToDateTimeUtc() },
            new() { ExternalId = $"SomeExternalId-organisation-match-5", Date = _fixture.Now.Plus(Duration.FromHours(5)).ToDateTimeUtc() },
        }, options => options
            .Including(e => e.ExternalId)
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
        var body = await response.Content.ReadFromJsonAsync<ICollection<InstitutionAccountTransactionDto>>();

        body.Should().BeEquivalentTo(new List<InstitutionAccountTransactionDto>()
        {
            new() { ExternalId = $"SomeExternalId-organisation-match-1", Date = _fixture.Now.Plus(Duration.FromHours(1)).ToDateTimeUtc() },
        }, options => options
            .Including(e => e.ExternalId)
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
                Filter = "ExternalId = SomeExternalId-organisation-match-0"
            });
        var body = await response.Content.ReadFromJsonAsync<ICollection<InstitutionAccountTransactionDto>>();

        // Assert
        body.Should().BeEquivalentTo(new List<InstitutionAccountTransactionDto>()
        {
            new() { ExternalId = $"SomeExternalId-organisation-match-0" },
        }, options => options.Including(e => e.ExternalId));

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

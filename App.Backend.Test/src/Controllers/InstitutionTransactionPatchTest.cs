using System.Net;
using System.Net.Http.Json;
using App.Backend.Controllers;
using App.Backend.Dto;
using App.Lib.Data.Entity;
using App.Lib.Test.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NodaTime;
using Xunit.Sdk;

namespace App.Backend.Test.Controllers;

public class InstitutionTransactionPatchTest
{
    private readonly DatabasePool _databasePool;
    private readonly ILoggerProvider _loggerProvider;

    public InstitutionTransactionPatchTest(DatabasePool databasePool, ILoggerProvider loggerProvider)
    {
        _databasePool = databasePool;
        _loggerProvider = loggerProvider;
    }

    [Fact]
    public async Task Patch()
    {
        // Arrange
        var fixture = new InstitutionTransactionFixture(_databasePool, _loggerProvider);
        var category = new TransactionCategoryEntity()
        {
            Name = "Some Category",
            OrganisationId = fixture.OrganisationId,
            Group = TransactionCategoryGroup.Expense,
        };
        fixture.Database.SeedData(database =>
        {
            database.TransactionCategory.Add(category);
        });

        // Act
        var response = await fixture.Client.SendWithAuthAsync(
            new HttpRequestMessage(HttpMethod.Patch, $"{InstitutionTransactionListController.RouteBase}?id={fixture.TransactionA.Id}")
            {
                Content = JsonContent.Create(new InstitutionTransactionPatchDto()
                {
                    CategoryId = category.Id,
                }),
            });

        // Assert
        fixture.Database.WithData(database =>
        {
            var transaction = database.InstitutionAccountTransactions.Find(fixture.TransactionA.Id)
                ?? throw FailException.ForFailure("Transaction not found");

            transaction.CategoryId.Should().Be(category.Id);
        });
    }

    [Fact]
    public async Task Patch_Category_OutsideOrganisation()
    {
        // Arrange
        var fixture = new InstitutionTransactionFixture(_databasePool, _loggerProvider);
        var category = new TransactionCategoryEntity()
        {
            Name = "Some Category",
            OrganisationId = fixture.AltOrganisationId,
            Group = TransactionCategoryGroup.Expense,
        };
        fixture.Database.SeedData(database =>
        {
            database.TransactionCategory.Add(category);
        });

        // Act
        var response = await fixture.Client.SendWithAuthAsync(
            new HttpRequestMessage(HttpMethod.Patch, $"{InstitutionTransactionListController.RouteBase}?id={fixture.TransactionA.Id}")
            {
                Content = JsonContent.Create(new InstitutionTransactionPatchDto()
                {
                    CategoryId = category.Id,
                }),
            });

        // Assert
        await response.AssertProblemDetails(HttpStatusCode.BadRequest, new ProblemDetails()
        {
            Title = "Category not found",
            Detail = "The category does not exist or is not accessible",
            Status = (int)HttpStatusCode.BadRequest,
        });
    }

    [Fact]
    public async Task Patch_Transaction_OutsideOrganisation()
    {
        // Arrange
        var fixture = new InstitutionTransactionFixture(_databasePool, _loggerProvider);
        var categoryEntity = new TransactionCategoryEntity()
        {
            Name = "Some Category",
            OrganisationId = fixture.OrganisationId,
            Group = TransactionCategoryGroup.Expense,
        };
        var institutionConnectionEntity = new InstitutionConnectionEntity()
        {
            ExternalId = "SomeExternalId-organisation-missmatch-A",
            ConnectUrl = new Uri($"https://www.example-organisation-match-0.com/"),
            InstitutionId = fixture.InstitutionEntityA.Id,
            OrganisationId = fixture.AltOrganisationId,
        };
        var institutionAccountEntity = new InstitutionAccountEntity()
        {
            ExternalId = "SomeExternalId-organisation-missmatch-A",
            Iban = "NL00BANK0123456789",
            InstitutionConnectionId = institutionConnectionEntity.Id,
        };
        var transactionEntity = new InstitutionAccountTransactionEntity()
        {
            ExternalId = $"account-a",
            Amount = 100,
            Currency = "EUR",
            Account = institutionAccountEntity,
            UnstructuredInformation = "Some unstructured information",
            Date = Instant.FromUtc(2023, 12, 1, 10, 0),
        };
        fixture.Database.SeedData(database =>
        {
            database.TransactionCategory.Add(categoryEntity);
            database.InstitutionConnections.Add(institutionConnectionEntity);
            database.InstitutionAccounts.Add(institutionAccountEntity);
            database.InstitutionAccountTransactions.Add(transactionEntity);
        });

        // Act
        var response = await fixture.Client.SendWithAuthAsync(
            new HttpRequestMessage(HttpMethod.Patch, $"{InstitutionTransactionListController.RouteBase}?id={institutionAccountEntity.Id}")
            {
                Content = JsonContent.Create(new InstitutionTransactionPatchDto()
                {
                    CategoryId = categoryEntity.Id,
                }),
            });

        // Assert
        await response.AssertProblemDetails(HttpStatusCode.BadRequest, new ProblemDetails()
        {
            Title = "Transaction not found",
            Detail = "The transaction does not exist or is not accessible",
            Status = (int)HttpStatusCode.BadRequest,
        });
    }

    [Fact]
    public async Task Patch_Category_NotFound()
    {
        // Arrange
        var fixture = new InstitutionTransactionFixture(_databasePool, _loggerProvider);

        // Act
        var response = await fixture.Client.SendWithAuthAsync(
            new HttpRequestMessage(HttpMethod.Patch, $"{InstitutionTransactionListController.RouteBase}?id={fixture.TransactionA.Id}")
            {
                Content = JsonContent.Create(new InstitutionTransactionPatchDto()
                {
                    CategoryId = new Guid("75df7893-b75b-4731-b346-73678e27f00d"),
                }),
            });

        // Assert
        await response.AssertProblemDetails(HttpStatusCode.BadRequest, new ProblemDetails()
        {
            Title = "Category not found",
            Detail = "The category does not exist or is not accessible",
            Status = (int)HttpStatusCode.BadRequest,
        });
    }

    [Fact]
    public async Task Patch_Transaction_NotFound()
    {
        // Arrange
        var fixture = new InstitutionTransactionFixture(_databasePool, _loggerProvider);
        var categoryEntity = new TransactionCategoryEntity()
        {
            Name = "Some Category",
            OrganisationId = fixture.OrganisationId,
            Group = TransactionCategoryGroup.Expense,
        };
        fixture.Database.SeedData(database =>
        {
            database.TransactionCategory.Add(categoryEntity);
        });

        // Act
        var response = await fixture.Client.SendWithAuthAsync(
            new HttpRequestMessage(HttpMethod.Patch, $"{InstitutionTransactionListController.RouteBase}?id=c66c395b-c614-4481-b392-6f7d9231821f")
            {
                Content = JsonContent.Create(new InstitutionTransactionPatchDto()
                {
                    CategoryId = categoryEntity.Id,
                }),
            });

        // Assert
        await response.AssertProblemDetails(HttpStatusCode.BadRequest, new ProblemDetails()
        {
            Title = "Transaction not found",
            Detail = "The transaction does not exist or is not accessible",
            Status = (int)HttpStatusCode.BadRequest,
        });
    }
}
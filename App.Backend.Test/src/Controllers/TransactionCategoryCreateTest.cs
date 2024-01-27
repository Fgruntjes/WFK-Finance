
using System.Net;
using System.Net.Http.Json;
using App.Backend.Controllers;
using App.Backend.Dto;
using App.Lib.Test;
using App.Lib.Data.Entity;
using App.Lib.Data.Exception;
using App.TransactionCategory.Exception;
using App.TransactionCategory.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace App.Backend.Test.Controllers;

public class TransactionCategoryCreateTest : IClassFixture<TransactionCategoryFixture>
{
    private readonly TransactionCategoryFixture _fixture;

    public TransactionCategoryCreateTest(TransactionCategoryFixture fixture)
    {
        _fixture = fixture;
    }


    [Fact]
    public async Task Post()
    {
        // Arrange
        _fixture.Services.WithMock<ITransactionCategoryService>((mock) =>
        {
            mock.Setup(x => x.CreateAsync("Test", TransactionCategoryGroup.Expense, null, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TransactionCategoryEntity()
                {
                    Id = new Guid("581fa5df-a0f2-499e-add5-c11bb7d66a59"),
                    Name = "Test",
                    Group = TransactionCategoryGroup.Expense,
                    OrganisationId = _fixture.OrganisationId,
                    SortOrder = 10,
                });
        });

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(new HttpRequestMessage(HttpMethod.Post, TransactionCategoryListController.RouteBase)
        {
            Content = JsonContent.Create(new TransactionCategoryInputDto()
            {
                Name = "Test",
                Group = TransactionCategoryGroup.Expense,
                SortOrder = 10,
            })
        });
        var content = await response.Content.ReadFromJsonAsync<TransactionCategoryDto>();

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Created);
        content.Should().BeEquivalentTo(new TransactionCategoryDto()
        {
            Id = new Guid("581fa5df-a0f2-499e-add5-c11bb7d66a59"),
            Name = "Test",
            Group = TransactionCategoryGroup.Expense,
            SortOrder = 10,
        });
    }

    [Fact]
    public async Task Post_UniqueConstraintException()
    {
        // Arrange
        _fixture.Services.WithMock<ITransactionCategoryService>((mock) =>
        {
            mock.Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<TransactionCategoryGroup>(), null, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UniqueConstraintException());
        });

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(new HttpRequestMessage(HttpMethod.Post, TransactionCategoryListController.RouteBase)
        {
            Content = JsonContent.Create(new TransactionCategoryInputDto()
            {
                Name = "Test",
                Group = TransactionCategoryGroup.Expense,
            })
        });

        // Assert
        await response.AssertProblemDetails(HttpStatusCode.Conflict, new ProblemDetails
        {
            Title = "An error occurred",
            Detail = "Request conflicted with existing entities.",
            Status = (int)HttpStatusCode.Conflict,
        });
    }

    [Fact]
    public async Task Post_ParentCategoryNotFoundException()
    {
        // Arrange
        var parentId = new Guid("ae3c5f2a-064f-4343-be74-08980ee6d952");
        _fixture.Services.WithMock<ITransactionCategoryService>((mock) =>
        {
            mock.Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<TransactionCategoryGroup>(), parentId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new CategoryNotFoundException(parentId));
        });

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(new HttpRequestMessage(HttpMethod.Post, TransactionCategoryListController.RouteBase)
        {
            Content = JsonContent.Create(new TransactionCategoryInputDto()
            {
                Name = "Test",
                Group = TransactionCategoryGroup.Expense,
                ParentId = parentId,
            })
        });

        // Assert
        await response.AssertProblemDetails(HttpStatusCode.BadRequest, new ProblemDetails
        {
            Title = "An error occurred",
            Detail = "Id {Id} not found",
            Status = (int)HttpStatusCode.BadRequest,
            Extensions =
            {
                { "Id", parentId }
            }
        });
    }
}

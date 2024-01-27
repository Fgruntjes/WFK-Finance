using System.Net;
using System.Net.Http.Json;
using App.Backend.Controllers;
using App.Backend.Dto;
using App.Lib.Data.Entity;
using App.TransactionCategory.Interface;
using App.Lib.Test;
using Microsoft.AspNetCore.Mvc;
using App.TransactionCategory.Exception;
using Moq;
using App.Lib.Data.Exception;

namespace App.Backend.Test.Controllers;

public class TransactionCategoryPatchTest : IClassFixture<TransactionCategoryFixture>
{
    private readonly TransactionCategoryFixture _fixture;

    public TransactionCategoryPatchTest(TransactionCategoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Patch()
    {
        // Arrange
        var parentId = new Guid("0f0a62aa-8860-4d1d-ad6d-1722a6039faf");
        var returnEntity = new TransactionCategoryEntity()
        {
            Id = _fixture.TransactionCategoryEntity.Id,
            Name = "NewName",
            Group = TransactionCategoryGroup.Expense,
            ParentId = parentId,
            OrganisationId = _fixture.OrganisationId,
            SortOrder = 20,
        };
        _fixture.Services.WithMock<ITransactionCategoryService>((mock) =>
        {
            mock.Setup(x => x.UpdateAsync(_fixture.TransactionCategoryEntity.Id, "NewName", TransactionCategoryGroup.Expense, parentId, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnEntity);
        });

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(new HttpRequestMessage(
            HttpMethod.Put, $"{TransactionCategoryListController.RouteBase}/{_fixture.TransactionCategoryEntity.Id}")
        {
            Content = JsonContent.Create(new TransactionCategoryInputDto()
            {
                Name = "NewName",
                Group = TransactionCategoryGroup.Expense,
                ParentId = parentId,
                SortOrder = 20,
            })
        });
        var content = await response.Content.ReadFromJsonAsync<TransactionCategoryDto>();

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Created);
        content.Should().BeEquivalentTo(new TransactionCategoryDto()
        {
            Id = _fixture.TransactionCategoryEntity.Id,
            Name = "NewName",
            Group = TransactionCategoryGroup.Expense,
            ParentId = parentId,
            SortOrder = 20,
        });
    }

    [Fact]
    public async Task Patch_UniqueConstraintException()
    {
        // Arrange
        _fixture.Services.WithMock<ITransactionCategoryService>((mock) =>
        {
            mock.Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<TransactionCategoryGroup>(), null, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UniqueConstraintException());
        });

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(new HttpRequestMessage(
            HttpMethod.Put, $"{TransactionCategoryListController.RouteBase}/{_fixture.TransactionCategoryEntity.Id}")
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
    public async Task Patch_ParentCategoryNotFoundException()
    {
        // Arrange
        var parentId = new Guid("784ed104-cf82-4a13-b2b7-05b7cffb3d15");
        _fixture.Services.WithMock<ITransactionCategoryService>((mock) =>
        {
            mock.Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<TransactionCategoryGroup>(), null, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new CategoryNotFoundException(parentId));
        });

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(new HttpRequestMessage(
            HttpMethod.Put, $"{TransactionCategoryListController.RouteBase}/{_fixture.TransactionCategoryEntity.Id}")
        {
            Content = JsonContent.Create(new TransactionCategoryInputDto()
            {
                Name = "Test",
                Group = TransactionCategoryGroup.Expense,
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
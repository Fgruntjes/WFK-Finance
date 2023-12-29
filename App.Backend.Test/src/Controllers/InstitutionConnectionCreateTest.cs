using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using App.Backend.Dto;
using App.Lib.InstitutionConnection.Exception;
using App.Lib.InstitutionConnection.Service;
using App.Lib.Test;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionCreateTest : IClassFixture<InstitutionConnectionFixture>
{
    private readonly InstitutionConnectionFixture _fixture;

    public InstitutionConnectionCreateTest(InstitutionConnectionFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Success()
    {
        // Arrange
        var returnUri = new Uri("http://www.example.com/return");
        _fixture.Services.WithMock<IInstitutionConnectionCreateService>(mock =>
        {
            mock.Setup(e => e.Connect(
                    _fixture.InstitutionEntity.Id,
                    returnUri,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_fixture.InstitutionConnectionEntity);
        });

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(
            new HttpRequestMessage(HttpMethod.Post, "/institutionconnection")
            {
                Content = JsonContent.Create(new InstitutionConnectionCreate
                {
                    InstitutionId = _fixture.InstitutionEntity.Id,
                    ReturnUrl = returnUri
                })
            });
        var body = await response.Content.ReadFromJsonAsync<InstitutionConnection>();

        // Assert
        body.Should().BeEquivalentTo(new InstitutionConnection
        {
            Id = _fixture.InstitutionConnectionEntity.Id,
            ExternalId = _fixture.InstitutionConnectionEntity.ExternalId,
            InstitutionId = _fixture.InstitutionEntity.Id,
            ConnectUrl = _fixture.InstitutionConnectionEntity.ConnectUrl,
            Accounts = new List<InstitutionConnectionAccount>(),
        });
    }

    [Fact]
    public async Task BadRequest_InstitutionNotFoundException()
    {
        // Arrange
        var institutionId = new Guid("8665f9d8-fa7b-497d-b6c5-40fd7e68aefc");
        _fixture.Services.WithMock<IInstitutionConnectionCreateService>(mock =>
        {
            mock.Setup(e => e.Connect(
                   It.IsAny<Guid>(),
                    It.IsAny<Uri>(),
                    It.IsAny<CancellationToken>()))
                .Callback((Guid id, Uri _, CancellationToken _) => throw new InstitutionNotFoundException(id));
        });

        // Act
        var response = await _fixture.Client.SendWithAuthAsync(
            new HttpRequestMessage(HttpMethod.Post, "/institutionconnection")
            {
                Content = JsonContent.Create(new InstitutionConnectionCreate
                {
                    InstitutionId = institutionId,
                    ReturnUrl = new Uri("http://www.example.com/return")
                })
            });

        // Assert
        await response.AssertProblemDetails(HttpStatusCode.BadRequest, new ProblemDetails
        {
            Title = "An error occurred",
            Detail = "Institution {InstitutionId} not found",
            Status = (int)HttpStatusCode.BadRequest,
            Extensions =
            {
                { "InstitutionId", institutionId }
            }
        });
    }
}
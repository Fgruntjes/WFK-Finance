using Microsoft.EntityFrameworkCore;
using Moq;
using VMelnalksnis.NordigenDotNet.Requisitions;

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
        var requisitionsMock = new Mock<IRequisitionClient>();
        _fixture.NordigenClientMoq
            .SetupGet(c => c.Requisitions)
            .Returns(requisitionsMock.Object);

        var nordigenRequisitionResult = new Requisition
        {
            Link = new Uri("https://www.example.com/connect-url/nordigen-requisition"),
            Id = new Guid("2ee63e58-5e7e-4bf2-820f-646f85876bac"),
        };
        requisitionsMock
            .Setup(r => r.Post(It.IsAny<RequisitionCreation>()))
            .ReturnsAsync(nordigenRequisitionResult);

        // Act
        var result = await _fixture.ExecuteQuery(new
        {
            InstitutionId = _fixture.InstitutionEntity.Id,
            ReturnUrl = "http://www.example.com/return"
        });
        result.MatchSnapshot();

        // Assert
        _fixture.WithData(async conext =>
        {
            var connectionEntity = await conext.InstitutionConnections
                        .Where(e => e.ExternalId == nordigenRequisitionResult.Id.ToString())
                        .FirstAsync();

            connectionEntity.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task MissingInstitution()
    {
        var result = await _fixture.ExecuteQuery(new
        {
            InstitutionId = new Guid("63ee89f0-d929-4f13-9636-4efd23aee070"),
            ReturnUrl = "http://www.example.com/return"
        });
        result.MatchSnapshot(m => m.IgnoreField("errors[0].extensions.timestamp"));
    }
}
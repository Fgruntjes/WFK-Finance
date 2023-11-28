using Moq;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionCreateTest : IClassFixture<InstitutionConnectionCreateFixture>
{
    private readonly InstitutionConnectionCreateFixture _fixture;

    public InstitutionConnectionCreateTest(InstitutionConnectionCreateFixture fixture)
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
        var result = await _fixture.Server.ExecuteQuery(new
        {
            InstitutionId = _fixture.InstitutionEntity.Id,
            ReturnUrl = "http://www.example.com/return"
        });
        result.MatchSnapshot();

        // Assert
        _fixture.WithData(databaseContext =>
        {
            var connections = databaseContext.InstitutionConnections.ToList();
            databaseContext.InstitutionConnections
                .First(e => e.ExternalId == nordigenRequisitionResult.Id.ToString())
                .Should().NotBeNull();
        });
    }

    [Fact]
    public async Task MissingInstitution()
    {
        var result = await _fixture.Server.ExecuteQuery(new
        {
            InstitutionId = new Guid("63ee89f0-d929-4f13-9636-4efd23aee070"),
            ReturnUrl = "http://www.example.com/return"
        });
        result.MatchSnapshot(m => m.IgnoreField("errors[0].extensions.timestamp"));
    }

    [Fact]
    public async Task DifferentOrganisationPerUser()
    {
        var requisitionsMock = new Mock<IRequisitionClient>();
        _fixture.NordigenClientMoq
            .SetupGet(c => c.Requisitions)
            .Returns(requisitionsMock.Object);

        var nordigenRequisitionResultOne = new Requisition
        {
            Link = new Uri("https://www.example.com/connect-url/nordigen-requisition"),
            Id = new Guid("2ee63e58-5e7e-4bf2-820f-646f85876bac"),
        };
        var nordigenRequisitionResultTwo = new Requisition
        {
            Link = new Uri("https://www.example.com/connect-url/nordigen-requisition"),
            Id = new Guid("93228ed4-4e0b-409a-b564-d38395c4a045"),
        };
        requisitionsMock
            .SetupSequence(r => r.Post(It.IsAny<RequisitionCreation>()))
            .ReturnsAsync(nordigenRequisitionResultOne)
            .ReturnsAsync(nordigenRequisitionResultTwo);

        var connectVariables = new
        {
            InstitutionId = _fixture.InstitutionEntity.Id,
            ReturnUrl = "http://www.example.com/return"
        };

        const string userTwoId = "auth0|p7rruszw5d7xpga32royaql4";
        var userTwoServer = _fixture.CreateServer(userTwoId);

        // Act
        (await _fixture.Server.ExecuteQuery(connectVariables))
            .MatchSnapshot("InstitutionConnectionCreateTest.DifferentOrganisationPerUser.1.snap");
        (await userTwoServer.ExecuteQuery(connectVariables))
            .MatchSnapshot("InstitutionConnectionCreateTest.DifferentOrganisationPerUser.2.snap");

        // Assert
        _fixture.WithData(databaseContext =>
        {
            databaseContext.InstitutionConnections
                .Where(e => e.ExternalId == nordigenRequisitionResultOne.Id.ToString())
                .First(e => e.OrganisationId == _fixture.OrganisationId)
                .OrganisationId.Should().Be(_fixture.OrganisationId);

            var userTwoOrganisation = databaseContext.Organisations.First(e => e.Slug == userTwoId);
            databaseContext.InstitutionConnections
                .Where(e => e.ExternalId == nordigenRequisitionResultTwo.Id.ToString())
                .First(e => e.OrganisationId == userTwoOrganisation.Id)
                .OrganisationId.Should().Be(userTwoOrganisation.Id);
        });
    }
}
using App.Lib.Data;
using App.Lib.InstitutionConnection.Exception;
using App.Lib.InstitutionConnection.Service;
using App.Lib.Test;
using App.Lib.Test.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Lib.InstitutionConnection.Test.Service;

public class InstitutionConnectionCreateServiceTest
{
    private readonly DatabasePool _databasePool;
    private readonly ILoggerProvider _loggerProvider;

    public InstitutionConnectionCreateServiceTest(DatabasePool databasePool, ILoggerProvider loggerProvider)
    {
        _databasePool = databasePool;
        _loggerProvider = loggerProvider;
    }

    [Fact]
    public void Implementation()
    {
        // Arrange
        var fixture = new InstitutionConnectionCreateFixture(_databasePool, _loggerProvider);

        // Act
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionCreateService>();

        // Assert
        service.Should().BeOfType<InstitutionConnectionCreateService>();
    }

    [Fact]
    public async Task Success()
    {
        // Arrange
        var fixture = new InstitutionConnectionCreateFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionCreateService>();

        var nordigenRequisitionResult = new Requisition
        {
            Link = new Uri("https://www.example.com/connect-url/nordigen-requisition"),
            Id = new Guid("6a108a59-bdd5-4844-9691-22442cfe8649"),
        };
        fixture.Services.WithMock<IRequisitionClient>(mock =>
        {
            mock.Setup(r => r.Post(It.IsAny<RequisitionCreation>()))
                .ReturnsAsync(nordigenRequisitionResult);
        });

        // Act
        var result = await service.Connect(
            fixture.InstitutionEntity.Id,
            new Uri("http://www.example.com/return"));
        result.InstitutionId.Should().Be(fixture.InstitutionEntity.Id);

        // Assert
        fixture.Database.WithData(databaseContext =>
        {
            databaseContext.InstitutionConnections
                .First(e => e.ExternalId == nordigenRequisitionResult.Id.ToString())
                .Should().NotBeNull();
        });
    }

    [Fact]
    public async Task MissingInstitution()
    {
        // Arrange
        var fixture = new InstitutionConnectionCreateFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionCreateService>();

        var institutionId = new Guid("63ee89f0-d929-4f13-9636-4efd23aee070");

        // Act
        var act = async () => await service.Connect(
            institutionId,
            new Uri("http://www.example.com/return"));

        // Assert
        var exception = await Assert.ThrowsAsync<InstitutionNotFoundException>(act);
        exception.Data["InstitutionId"].Should().Be(institutionId);
    }

    [Fact]
    public async Task DifferentOrganisationPerUser()
    {
        var fixture = new InstitutionConnectionCreateFixture(_databasePool, _loggerProvider);
        var service = fixture.Services.GetRequiredService<IInstitutionConnectionCreateService>();

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
        fixture.Services.WithMock<IRequisitionClient>(mock =>
        {
            mock.SetupSequence(r => r.Post(It.IsAny<RequisitionCreation>()))
                .ReturnsAsync(nordigenRequisitionResultOne)
                .ReturnsAsync(nordigenRequisitionResultTwo);
        });
        fixture.Services.WithMock<IOrganisationIdProvider>(mock =>
        {
            mock.Reset();
            mock.SetupSequence(m => m.GetOrganisationId())
                .Returns(fixture.OrganisationId)
                .Returns(fixture.AltOrganisationId);
        });

        // Act
        var user1Response = await service.Connect(
            fixture.InstitutionEntity.Id,
            new Uri("http://www.example.com/return"));
        user1Response.OrganisationId.Should().Be(fixture.OrganisationId);
        var user2Response = await service.Connect(
            fixture.InstitutionEntity.Id,
            new Uri("http://www.example.com/return"));
        user2Response.OrganisationId.Should().Be(fixture.AltOrganisationId);

        // Assert
        fixture.Database.WithData(databaseContext =>
        {
            databaseContext.InstitutionConnections
                .Where(e => e.ExternalId == nordigenRequisitionResultOne.Id.ToString())
                .First(e => e.OrganisationId == fixture.OrganisationId)
                .OrganisationId.Should().Be(fixture.OrganisationId);

            var userTwoOrganisation = databaseContext.Organisations.First(e => e.Id == fixture.AltOrganisationId);
            databaseContext.InstitutionConnections
                .Where(e => e.ExternalId == nordigenRequisitionResultTwo.Id.ToString())
                .First(e => e.OrganisationId == userTwoOrganisation.Id)
                .OrganisationId.Should().Be(userTwoOrganisation.Id);
        });
    }
}
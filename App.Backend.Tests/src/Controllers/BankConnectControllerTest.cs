using System.Net;
using App.Backend.Data;
using App.Backend.Data.Entity;
using App.Backend.DTO;
using App.Backend.Tests.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Moq;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Backend.Tests.Controllers;

public class BankConnectControllerTest : TestFixture<Program>
{
    private Mock<IRequisitionClient> _mockedNordigenRequisitions;
    private Mock<INordigenClient> _mockedNordigenClient;

    public BankConnectControllerTest(TestApplicationFactory<Program> factory) : base(factory)
    {
        _mockedNordigenRequisitions = new Mock<IRequisitionClient>();
        _mockedNordigenClient = new Mock<INordigenClient>();
        _mockedNordigenClient
            .SetupGet(c => c.Requisitions)
            .Returns(_mockedNordigenRequisitions.Object);

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(s => s.AddTransient<INordigenClient>(_ => _mockedNordigenClient.Object));
        }).CreateClient();
    }

    [Fact]
    public async void Connect_CreateEntity()
    {
        var returnUrl = new Uri("https://example.com/return");
        var requisitionUrl = new Uri("https://example.com/requisition");
        var institutionId = "SomeInstitute";

        _mockedNordigenRequisitions
            .SetupAndVerify(
                r => r.Post(It.Is<RequisitionCreation>(r => r.InstitutionId == institutionId && r.Redirect == returnUrl)),
                Times.Once()
            )
            .ReturnsAsync(new Requisition { Link = requisitionUrl });

        var request = new BankConnectRequest
        {
            ReturnUrl = returnUrl,
            BankId = institutionId
        };

        var response = (await _client.PostAsJsonAsync("/Bank/Connect", request));
        response.Should().HaveStatusCode(HttpStatusCode.OK);

        (await response.Content.ReadFromJsonAsync<BankConnectResponse>())
            .Should()
            .BeEquivalentTo(new BankConnectResponse { Url = requisitionUrl });
    }

    [Fact]
    public async void Connect_UpdateEntity()
    {
        var returnUrl = new Uri("https://example.com/return");
        var requisitionUrl = new Uri("https://example.com/requisition");
        var institutionId = "SomeInstitute";

        await DatabaseContext.InstitutionConnections.InsertOneAsync(new InstitutionConnectionEntity
        {
            TenantId = new ObjectId((await AuthContext.GetTenant(TestAuthenticationHandler.DefaultExternalUserId)).Id),
            BankId = institutionId,
            ConnectUrl = requisitionUrl
        });

        var request = new BankConnectRequest
        {
            ReturnUrl = returnUrl,
            BankId = institutionId
        };

        var response = (await _client.PostAsJsonAsync("/Bank/Connect", request));
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<BankConnectResponse>())
            .Should()
            .BeEquivalentTo(new BankConnectResponse { Url = requisitionUrl });
    }
}
using App.Lib.InstitutionConnection.Exception;
using App.Lib.InstitutionConnection.Service;
using App.Lib.Test;
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
        const string returnUri = "http://www.example.com/return";
        _fixture.Services.WithMock<IInstitutionConnectionCreateService>(mock =>
        {
            mock.Setup(e => e.Connect(
                    _fixture.InstitutionEntity.Id,
                    It.Is<Uri>(url => url.ToString() == returnUri),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_fixture.InstitutionConnectionEntity);
        });

        // Act
        var result = await _fixture.Client.ExecuteQuery(new
        {
            InstitutionId = _fixture.InstitutionEntity.Id,
            ReturnUrl = returnUri
        });

        // Assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task BadRequest_InstitutionNotFoundException()
    {
        // Arrange
        _fixture.Services.WithMock<IInstitutionConnectionCreateService>(mock =>
        {
            mock.Setup(e => e.Connect(
                   It.IsAny<Guid>(),
                    It.IsAny<Uri>(),
                    It.IsAny<CancellationToken>()))
                .Callback((Guid institutionId, Uri _, CancellationToken _) => throw new InstitutionNotFoundException(institutionId));
        });

        // Act
        var result = await _fixture.Client.ExecuteQuery(new
        {
            InstitutionId = new Guid("8665f9d8-fa7b-497d-b6c5-40fd7e68aefc"),
            ReturnUrl = "http://www.example.com/return"
        });

        // Assert
        result.MatchSnapshot(m => m
            .IgnoreField("errors[0].extensions.timestamp")
            .IgnoreField("errors[0].extensions.exception"));
    }
}
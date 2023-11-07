namespace App.Backend.Test.Controllers;

public class InstitutionConnectionListTest : IClassFixture<InstitutionConnectionListFixture>
{
    private readonly InstitutionConnectionListFixture _fixture;

    public InstitutionConnectionListTest(InstitutionConnectionListFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WithoutSkipLimit()
    {
        var result = await _fixture.ExecuteQuery();
        result.MatchSnapshot();
    }

    [Fact]
    public async Task WithSkipLimit()
    {
        var result = await _fixture.ExecuteQuery(new { offset = 1, limit = 1 });
        result.MatchSnapshot();
    }
}
using App.Lib.Test;

namespace App.Backend.Test;

public class TestRequestOptions<TBody>
{
    public string? User { get; set; } = FunctionalTestFixture.TestUserId;
    public HttpMethod Method { get; set; } = null!;
    public TBody? Body;
}
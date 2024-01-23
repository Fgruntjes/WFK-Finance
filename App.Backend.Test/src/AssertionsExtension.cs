using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Test;

public static class AssertionsExtension
{
    public static async Task AssertProblemDetails(
        this HttpResponseMessage response,
        HttpStatusCode statusCode,
        ProblemDetails expectation)
    {
        response.StatusCode.Should().Be(statusCode);

        var responseBody = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        responseBody.Should().BeEquivalentTo(expectation, o => o.Excluding(p => p.Extensions));

        foreach (var extension in expectation.Extensions)
        {
            responseBody!.Extensions.Should().ContainKey(extension.Key);
            responseBody!.Extensions[extension.Key]!.ToString().Should().BeEquivalentTo(extension.Value!.ToString());
        }
    }
}
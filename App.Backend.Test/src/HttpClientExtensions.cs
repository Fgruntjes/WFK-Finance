using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using App.Backend.Test.Auth;
using App.Lib.Test;
using Xunit.Sdk;

namespace App.Backend.Test;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> GetWithAuthAsync(
        this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri,
        CancellationToken cancellationToken = default)
    {
        return await client.SendWithAuthAsync(
            new HttpRequestMessage(HttpMethod.Get, requestUri),
            cancellationToken);
    }

    public static async Task<TResponse> GetWithAuthAsync<TResponse>(
        this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri,
        CancellationToken cancellationToken = default)
    {
        var response = await client.GetWithAuthAsync(requestUri, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken)
            ?? throw FailException.ForFailure("Could not deserialize response value");
    }

    public static Task<HttpResponseMessage> SendWithAuthAsync(
        this HttpClient client,
        HttpRequestMessage message,
        CancellationToken cancellationToken = default)
    {
        message.Content ??= JsonContent.Create<object?>(null);
        message.Content.Headers.Add(TestAuthHandler.TestUserHeader, FunctionalTestFixture.TestUserId);
        return client.SendAsync(message, cancellationToken);
    }
}

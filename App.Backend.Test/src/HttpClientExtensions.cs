using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using App.Backend.Dto;
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
        return await client.GetWithAuthAsync(new Uri(requestUri), cancellationToken);
    }
    public static async Task<HttpResponseMessage> GetWithAuthAsync(
        this HttpClient client,
        Uri requestUri,
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
        return await client.GetWithAuthAsync<TResponse>(new Uri(requestUri), cancellationToken);
    }

    public static async Task<TResponse> GetWithAuthAsync<TResponse>(
        this HttpClient client,
        Uri requestUri,
        CancellationToken cancellationToken = default)
    {
        var response = await client.GetWithAuthAsync(requestUri, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken)
            ?? throw FailException.ForFailure("Could not deserialize response value");
    }

    public static async Task<HttpResponseMessage> GetListWithAuthAsync(
        this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri,
        RangeParameter? range = null,
        SortOrder? sort = null,
        FilterParameter? filter = null,
        CancellationToken cancellationToken = default)
    {
        var uri = new Uri(requestUri);
        var query = HttpUtility.ParseQueryString(uri.Query);

        if (filter != null)
            query["filter"] = JsonSerializer.Serialize(filter, JsonOptions.Options);
        if (sort != null)
            query["sort"] = JsonSerializer.Serialize(sort, JsonOptions.Options);
        if (range != null)
            query["range"] = $"[{range.Start},{range.End}]";

        uri = new UriBuilder(uri) { Query = query.ToString() }.Uri;
        return await client.GetWithAuthAsync(uri, cancellationToken);
    }

    public static async Task<ICollection<TResponse>> GetListWithAuthAsync<TResponse>(
        this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri,
        FilterParameter? filter = null,
        SortOrder? sort = null,
        RangeParameter? range = null,
        CancellationToken cancellationToken = default)
    {
        var response = await client.GetListWithAuthAsync(requestUri, range, sort, filter, cancellationToken);

        return await response.Content.ReadFromJsonAsync<ICollection<TResponse>>(cancellationToken: cancellationToken)
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

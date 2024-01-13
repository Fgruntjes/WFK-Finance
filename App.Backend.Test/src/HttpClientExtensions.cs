using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using App.Backend.Test.Auth;
using App.Lib.Test;
using Gridify;
using Xunit.Sdk;

namespace App.Backend.Test;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

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

        return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken: cancellationToken)
            ?? throw FailException.ForFailure("Could not deserialize response value");
    }

    public static async Task<HttpResponseMessage> GetListWithAuthAsync(
        this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri,
        GridifyQuery? query = null,
        CancellationToken cancellationToken = default)
    {
        var uri = new Uri(requestUri);
        var httpQuery = HttpUtility.ParseQueryString(uri.Query);

        if (query?.Filter != null)
        {
            httpQuery["filter"] = query.Filter;
        }

        if (query?.OrderBy != null)
        {
            httpQuery["orderBy"] = query.OrderBy;
        }
        else
        {
            httpQuery["orderBy"] = "createdAt asc";
        }

        if (query != null && query.Page != 0)
        {
            httpQuery["page"] = query.Page.ToString();
        }

        if (query != null && query.PageSize != 0)
        {
            httpQuery["pageSize"] = query.PageSize.ToString();
        }
        else
        {
            httpQuery["pageSize"] = "25";
        }


        uri = new UriBuilder(uri) { Query = httpQuery.ToString() }.Uri;
        return await client.GetWithAuthAsync(uri, cancellationToken);
    }

    public static async Task<ICollection<TResponse>> GetListWithAuthAsync<TResponse>(
        this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri,
        GridifyQuery? query = null,
        CancellationToken cancellationToken = default)
    {
        var response = await client.GetListWithAuthAsync(requestUri, query, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<ICollection<TResponse>>(
            _jsonOptions,
            cancellationToken: cancellationToken);
        return result
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

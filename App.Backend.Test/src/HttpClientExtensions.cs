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
        string userId = FunctionalTestFixture.TestUserId,
        CancellationToken cancellationToken = default)
    {
        return await client.GetWithAuthAsync(new Uri(requestUri), userId, cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetWithAuthAsync(
        this HttpClient client,
        Uri requestUri,
        string userId = FunctionalTestFixture.TestUserId,
        CancellationToken cancellationToken = default)
    {
        return await client.SendWithAuthAsync(
            new HttpRequestMessage(HttpMethod.Get, requestUri),
            userId,
            cancellationToken);
    }

    public static async Task<TResponse> GetWithAuthAsync<TResponse>(
        this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri,
        string userId = FunctionalTestFixture.TestUserId,
        CancellationToken cancellationToken = default)
    {
        return await client.GetWithAuthAsync<TResponse>(new Uri(requestUri), userId, cancellationToken);
    }

    public static async Task<TResponse> GetWithAuthAsync<TResponse>(
        this HttpClient client,
        Uri requestUri,
        string userId = FunctionalTestFixture.TestUserId,
        CancellationToken cancellationToken = default)
    {
        var response = await client.GetWithAuthAsync(requestUri, userId, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken: cancellationToken)
            ?? throw FailException.ForFailure("Could not deserialize response value");
    }

    public static async Task<HttpResponseMessage> GetListWithAuthAsync(
        this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri,
        GridifyQuery? query = null,
        string userId = FunctionalTestFixture.TestUserId,
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
        return await client.GetWithAuthAsync(uri, userId, cancellationToken);
    }

    public static async Task<ICollection<TResponse>> GetListWithAuthAsync<TResponse>(
        this HttpClient client,
        [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri,
        GridifyQuery? query = null,
        string userId = FunctionalTestFixture.TestUserId,
        CancellationToken cancellationToken = default)
    {
        var response = await client.GetListWithAuthAsync(requestUri, query, userId, cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<ICollection<TResponse>>(
            _jsonOptions,
            cancellationToken: cancellationToken);
        return result
            ?? throw FailException.ForFailure("Could not deserialize response value");
    }

    public static Task<HttpResponseMessage> SendWithAuthAsync(
        this HttpClient client,
        HttpRequestMessage message,
        string userId = FunctionalTestFixture.TestUserId,
        CancellationToken cancellationToken = default)
    {
        message.Content ??= JsonContent.Create<object?>(null);
        message.Content.Headers.Add(TestAuthHandler.TestUserHeader, userId);
        return client.SendAsync(message, cancellationToken);
    }

    public static async Task<T> ReadFromJsonAsync<T>(
        this HttpContent content,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= _jsonOptions;
        return await HttpContentJsonExtensions.ReadFromJsonAsync<T>(content, options, cancellationToken)
            ?? throw FailException.ForFailure("Could not deserialize response value");
    }
}

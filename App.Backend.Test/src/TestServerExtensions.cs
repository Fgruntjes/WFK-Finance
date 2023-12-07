using System.Text;
using App.Backend.Test.Auth;
using Newtonsoft.Json;
using Snapshooter.Core;

namespace App.Backend.Test;

public static class HttpClientExtensions
{
    private static readonly ISnapshotFullNameReader _testNameResolver = new XunitSnapshotFullNameReader();

    public static async Task<string> ExecuteQuery(
        this HttpClient client,
        string? queryFile,
        object? variables = null,
        string? user = AppFixture.TestUserId)
    {
        var queryObject = new
        {
            query = LoadQueryFile(queryFile),
            variables
        };
        var json = JsonConvert.SerializeObject(queryObject);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        if (user != null)
        {
            content.Headers.Add(TestAuthHandler.TestUserHeader, user);
        }

        var response = await client.PostAsync("/graphql", content);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> ExecuteQuery(
        this HttpClient client,
        object? variables = null,
        string? user = AppFixture.TestUserId)
    {
        return await client.ExecuteQuery(null, variables, user);
    }


    private static string LoadQueryFile(string? queryFile)
    {
        var queryFullName = _testNameResolver.ReadSnapshotFullName();
        queryFile ??= Path.Combine(queryFullName.FolderPath, "__graphql__", queryFullName.Filename + ".graphql");
        if (!File.Exists(queryFile))
        {
            File.Create(queryFile).Dispose();
        }

        var queryText = File.ReadAllText(queryFile);
        return queryText;
    }

}

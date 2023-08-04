using System.Collections.Generic;
using System.Linq;
using App.Deploy.Component;
using App.Deploy.Components;
using Pulumi;

namespace App.Deploy;

internal class AppStack : Stack
{
    public AppStack()
    {
        var config = new AppConfig();
        var authScopes = new Dictionary<string, string>() {
            {"bankacounts:read", "Read bankaccounts"},
            {"bankacounts:connect", "Connect bankaccount"},
        };
        var authScopeString = authScopes.Select(kvp => kvp.Key).Aggregate((a, b) => $"{a} {b}");

        var auth = new AuthComponent(
            $"{config.Environment}-auth",
            new()
            {
                Environment = config.Environment,
                Scopes = authScopes,
            },
            new ComponentResourceOptions { Parent = this });

        var database = new DatabaseComponent(
            $"{config.Environment}-database",
            new()
            {
                Environment = config.Environment,
                GoogleRegion = config.GoogleRegion,
                MongoDbProjectId = config.MongoDbProjectId
            },
            new ComponentResourceOptions { Parent = this });

        var backend = new BackendComponent(
            $"{config.Environment}-backend",
            new()
            {
                AppEnvironment = config.Environment,
                AppVersion = config.Version,
                AuthDomain = config.Auth0Domain,
                AuthAudience = auth.PublicApiAudience,
                AuthScope = authScopeString,
                GoogleRegion = config.GoogleRegion,
                GoogleProjectSlug = config.GoogleProjectSlug,
                NordigenSecretId = config.NordigenSecretId,
                NordigenSecretKey = config.NordigenSecretKey,
                DatabaseConnectionString = database.ConnectionString,
                DatabaseName = database.DatabaseName,
            },
            new ComponentResourceOptions { Parent = this });

        new FrontendComponent(
            $"{config.Environment}-frontend",
            new()
            {
                Environment = config.Environment,
                AuthDomain = config.Auth0Domain,
                AuthAudience = auth.PublicApiAudience,
                AuthScope = authScopeString,
                ApiUrl = backend.BaseUrl,
                GoogleProjectSlug = config.GoogleProjectSlug,
                CloudflareAccountId = config.CloudflareAccountId,
                CloudflareApiToken = config.CloudflareApiToken,
            },
            new ComponentResourceOptions { Parent = this });
    }
}

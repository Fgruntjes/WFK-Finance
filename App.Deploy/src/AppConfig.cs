using Pulumi;

namespace App.Deploy;

internal class AppConfig
{
    private Config _config;

    public string Environment => _config.Require("environment");
    public string MongoDbProjectId => _config.Require("mongodb_project_id");
    public string GoogleRegion => _config.Require("google_region");
    public string Auth0Domain => _config.Require("auth0_domain");

    public AppConfig()
    {
        _config = new Config("app");
    }
}
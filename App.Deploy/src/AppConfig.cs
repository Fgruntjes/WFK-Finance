using Pulumi;

namespace App.Deploy;

internal class AppConfig
{
    private Config _config;

    public string Environment => _config.Require("environment");
    public string Version => _config.Require("version");
    public string MongoDbProjectId => _config.Require("mongodb_project_id");
    public string GoogleRegion => _config.Require("google_region");
    public string GoogleProjectSlug => _config.Require("google_project_slug");
    public string Auth0Domain => _config.Require("auth0_domain");
    public string NordigenSecretId => _config.Require("nordigen_secret_id");
    public string NordigenSecretKey => _config.Require("nordigen_secret_key");

    public AppConfig()
    {
        _config = new Config("app");
    }
}
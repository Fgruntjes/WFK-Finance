using System.Collections;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Mongodbatlas;
using Pulumi.Mongodbatlas.Inputs;
using Pulumi.Random;

namespace App.Deploy.Component;

public class DatabaseComponent : ComponentResource
{
    private IDictionary<string, string> RegionMap = new Dictionary<string, string>() {
        {"europe-west1", "WESTERN_EUROPE"}
    };

    [Output]
    public Output<string> ConnectionString { get; internal set; } = null!;

    public DatabaseComponent(
        string name,
        DatabaseComponentArgs args,
        ComponentResourceOptions opts
    ) : base("app:database", name, opts)
    {
        var server = new ServerlessInstance(
            name,
            new ServerlessInstanceArgs
            {
                ProjectId = args.MongoDbProjectId,
                ProviderSettingsBackingProviderName = "GCP",
                ProviderSettingsProviderName = "SERVERLESS",
                ProviderSettingsRegionName = args.GoogleRegion.Apply(region => RegionMap[region]),
            },
            new CustomResourceOptions { Parent = this });


        new ProjectIpAccessList(name, new()
        {
            Comment = "Public access",
            CidrBlock = "0.0.0.0/0",
            ProjectId = args.MongoDbProjectId,
        }, new CustomResourceOptions { Parent = this });

        var password = new RandomPassword($"{name}-password", new()
        {
            Length = 20,
            Special = false,
        }, new CustomResourceOptions { Parent = this });

        var user = new DatabaseUser($"{name}-admin", new()
        {
            ProjectId = args.MongoDbProjectId,
            Scopes = new[]
            {
                new DatabaseUserScopeArgs
                {
                    Name = args.Environment,
                    Type = "CLUSTER",
                }
            },
            Roles = new[]
            {
                new DatabaseUserRoleArgs
                {
                    DatabaseName = "admin",
                    RoleName = "atlasAdmin",
                },
            },
            AuthDatabaseName = "admin",
            Username = "admin",
            Password = password.Result,
        }, new CustomResourceOptions { Parent = this });

        ConnectionString = server.ConnectionStringsStandardSrv
            .Apply(str => user.Username
                .Apply(username => user.Password
                    .Apply(password => str.Replace("mongodb+srv://", $"mongodb+srv://{username}:{password}@"))));

        RegisterOutputs();
    }
}
using System.Collections.Generic;
using App.Deploy.Component;
using App.Deploy.Components;
using Pulumi;

namespace App.Deploy.Stacks;

internal class AppStack : Stack
{
    public AppStack()
    {
        var config = new AppConfig();

        new Auth0Component(
            $"{config.Environment}-auth",
            new()
            {
                Environment = config.Environment,
                Scopes = new Dictionary<string, string>() {
                    {"function.integrations", "Connections to external systems"}
                }
            },
            new ComponentResourceOptions { Parent = this });

        new DatabaseComponent(
            $"{config.Environment}-database",
            new()
            {
                Environment = config.Environment,
                GoogleRegion = config.GoogleRegion,
                MongoDbProjectId = config.MongoDbProjectId
            },
            new ComponentResourceOptions { Parent = this });
    }
}

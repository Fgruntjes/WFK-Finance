using System;
using System.Collections.Generic;
using App.Deploy.Components;
using App.Deploy.Utils;
using Pulumi;

namespace App.Deploy.Component;

public class BackendComponent : ComponentResource
{
    public Output<string> BaseUrl { get; private set; }

    public BackendComponent(
        string name,
        BackendComponentArgs args,
        ComponentResourceOptions opts
    ) : base("app:frontend", name, opts)
    {
        // So "npm run dev" use the deploy values
        EnvFileWriter.Write("../App.Backend/.local.env", new InputMap<string>() {
            {"Auth0__Domain", args.AuthDomain},
            {"Auth0__Audience", args.AuthAudience},
            {"Auth0__Scope", args.AuthScope},
            {"Nordigen__SecretId", args.NordigenSecretId},
            {"Nordigen__SecretKey", args.NordigenSecretKey},
            {"Database__ConnectionString", args.DatabaseConnectionString},
            {"Database__Name", args.DatabaseName},
        });

        BaseUrl = Output.Create("http://localhost:5000");
        RegisterOutputs();
    }
}

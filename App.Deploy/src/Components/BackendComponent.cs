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
        var authClient = new AuthClient(
            $"{name}-authclient",
            new()
            {
                Environment = args.Environment,
            },
            new ComponentResourceOptions { Parent = this });

        // So "npm run dev" use the deploy values
        EnvFileWriter.Write("../App.Backend/.local.env", new InputMap<string>() {
            {"Auth0__Domain", args.AuthDomain},
            {"Auth0__Audience", args.AuthAudience},
            {"Auth0__Scope", args.AuthScope},
            {"Auth0__ClientId", authClient.ClientId},
            {"Auth0__ClientSecret", authClient.ClientSecret},
            {"Nordigen__SecretId", args.NordigenSecretId},
            {"Nordigen__SecretKey", args.NordigenSecretKey},
            {"Database__ConnectionString", args.DatabaseConnectionString},
            {"Database__DatabaseName", args.DatabaseName},
        });

        BaseUrl = Output.Create("http://localhost:5000");
        RegisterOutputs();
    }
}

using System.Collections.Generic;
using App.Deploy.Components;
using App.Deploy.Utils;
using Pulumi;

namespace App.Deploy.Component;

public class FrontendComponent : ComponentResource
{
    public FrontendComponent(
        string name,
        FrontendComponentArgs args,
        ComponentResourceOptions opts
    ) : base("app:frontend", name, opts)
    {
        var authClient = new AuthClient(
            $"{name}-authclient",
            new()
            {
                Environment = args.Environment,
                FrontendUrl = args.Environment
                    .Apply(environment => environment == "dev"
                        ? "http://localhost:3000"
                        : "https://app.example.com"),
            },
            new ComponentResourceOptions { Parent = this });

        // So "npm run dev" use the deploy values
        EnvFileWriter.Write("../frontend/.env.local", new InputMap<string>() {
            {"AUTH0_DOMAIN", args.AuthDomain},
            {"AUTH0_AUDIENCE", args.AuthAudience},
            {"AUTH0_SCOPE", args.AuthScope},
            {"AUTH0_CLIENT_ID", authClient.ClientId},
        });
        RegisterOutputs();
    }
}

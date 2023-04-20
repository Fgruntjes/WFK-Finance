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
                FrontendUrls = args.Environment
                    .Apply(environment => environment == "dev"
                        ? new string[] {
                            "http://localhost:3000",
                            "http://localhost:5000",
                        }
                        : new string[] {
                            "https://example.com",
                        }),
                CallbackUrls = args.Environment
                    .Apply(environment => environment == "dev"
                        ? new string[] {
                            "http://localhost:3000",
                            "http://localhost:5000/swagger/oauth2-redirect.html",
                        }
                        : new string[] {
                            "https://example.com",
                        }),
            },
            new ComponentResourceOptions { Parent = this });

        // So "npm run dev" use the deploy values
        EnvFileWriter.Write("../frontend/.env.local", new InputMap<string>() {
            {"AUTH0_DOMAIN", args.AuthDomain},
            {"AUTH0_AUDIENCE", args.AuthAudience},
            {"AUTH0_SCOPE", args.AuthScope},
            {"AUTH0_CLIENT_ID", authClient.ClientId},
            {"APP_API_BASE_PATH", args.ApiUrl},
        });
        RegisterOutputs();
    }
}

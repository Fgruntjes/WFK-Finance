using System;
using System.Linq;
using System.Net.Mime;
using Pulumi;
using Pulumi.Auth0;
using Pulumi.Auth0.Inputs;

namespace App.Deploy.Components;

class AuthClient : ComponentResource
{
    public Output<string> ClientId { get; private set; } = null!;
    public Output<string> ClientSecret { get; private set; } = null!;

    public AuthClient(string name, AuthClientArgs args, ComponentResourceOptions? options = null)
        : base("app:auth0", name, options)
    {
        var isFrontend = args.FrontendUrl != null;
        var appUrls = args.FrontendUrl != null
            ? new[] { args.FrontendUrl }
            : new InputList<string>();

        var client = new Client(
            $"{name}-authclient",
            new()
            {
                AppType = isFrontend ? "spa" : "non_interactive",
                TokenEndpointAuthMethod = "none",
                CrossOriginAuth = false,
                WebOrigins = appUrls,
                Callbacks = appUrls,
                AllowedLogoutUrls = appUrls,
                JwtConfiguration = new ClientJwtConfigurationArgs()
                {
                    Alg = "RS256",
                },
                ClientMetadata = new()
                {
                    {"environment", args.Environment}
                },
            },
            new CustomResourceOptions { Parent = this });

        ClientId = client.ClientId;
        ClientSecret = client.ClientSecret;
        RegisterOutputs();
    }
}
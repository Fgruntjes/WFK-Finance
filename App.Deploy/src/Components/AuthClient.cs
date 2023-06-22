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
        var isFrontend = args.FrontendUrls != null;

        var client = new Client(
            $"{name}-authclient",
            new()
            {
                AppType = isFrontend ? "spa" : "non_interactive",
                TokenEndpointAuthMethod = "none",
                CrossOriginAuth = false,
                Callbacks = args.CallbackUrls ?? args.FrontendUrls ?? Array.Empty<string>(),
                WebOrigins = args.FrontendUrls ?? Array.Empty<string>(),
                AllowedLogoutUrls = args.FrontendUrls ?? Array.Empty<string>(),
                OidcConformant = true,
                RefreshToken = new ClientRefreshTokenArgs()
                {
                    RotationType = "rotating",
                    ExpirationType = "expiring",
                    TokenLifetime = 86400 * 365,
                },
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
using App.Deploy.Components;
using App.Deploy.Utils;
using Pulumi;
using Pulumi.Cloudflare;
using Pulumi.Command.Local;

namespace App.Deploy.Component;

public class FrontendComponent : ComponentResource
{
    [Output]
    public Output<string> FrontendUrl { get; internal set; } = null!;
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
                FrontendUrls = args.GoogleProjectSlug.Apply(googleProjectSlug =>
                    args.Environment.Apply(environment => environment == "dev"
                        ? new string[] {
                            "http://localhost:3000",
                            "http://localhost:8080",
                            $"https://{environment}.{googleProjectSlug}.pages.dev",
                        }
                        : new string[] {
                            $"https://{environment}.{googleProjectSlug}.pages.dev",
                        })),
                CallbackUrls = args.GoogleProjectSlug.Apply(googleProjectSlug =>
                    args.Environment.Apply(environment => environment == "dev"
                        ? new string[] {
                            "http://localhost:3000",
                            "http://localhost:8080/swagger/oauth2-redirect.html",
                            $"https://{environment}.{googleProjectSlug}.pages.dev",
                        }
                        : new string[] {
                            $"https://{environment}.{googleProjectSlug}.pages.dev",
                        })),
            },
            new ComponentResourceOptions { Parent = this });

        var cfProject = new PagesProject(
            $"{name}-pages-project",
            new()
            {
                AccountId = args.CloudflareAccountId,
                Name = args.GoogleProjectSlug,
                ProductionBranch = "main",
            });

        var wranglerPush = new Command(
            $"{name}-deploy",
            new()
            {
                Environment = {
                    { "CLOUDFLARE_API_TOKEN", args.CloudflareApiToken },
                    { "CLOUDFLARE_ACCOUNT_ID", args.CloudflareAccountId },
                },
                Create = args.Environment.Apply(environment =>
                    args.GoogleProjectSlug.Apply(googleProjectSlug =>
                        string.Join(' ', new string[] {
                            "npx",
                            "wrangler",
                            "pages",
                            "publish",
                            "../frontend/build",
                            $"--project-name={googleProjectSlug}",
                            $"--branch={environment}",
                            "--commit-dirty=true",
                        })))
            });

        // So "npm run dev" use the deploy values
        EnvFileWriter.Write("../frontend/.env.local", new InputMap<string>() {
            {"AUTH0_DOMAIN", args.AuthDomain},
            {"AUTH0_AUDIENCE", args.AuthAudience},
            {"AUTH0_SCOPE", args.AuthScope},
            {"AUTH0_CLIENT_ID", authClient.ClientId},
            {"APP_API_URI", args.ApiUrl},
        });

        FrontendUrl = args.GoogleProjectSlug.Apply(googleProjectSlug =>
            args.Environment.Apply(environment => $"https://{environment}.{googleProjectSlug}.pages.dev"));

        RegisterOutputs();
    }
}

using Pulumi;

namespace App.Deploy.Component;

public class FrontendComponentArgs
{
    public Input<string> Environment { get; init; } = null!;
    public Input<string> AuthDomain { get; init; } = null!;
    public Input<string> AuthAudience { get; init; } = null!;
    public Input<string> AuthScope { get; init; } = null!;
    public Input<string> ApiUrl { get; init; } = null!;
    public Input<string> CloudflareApiToken { get; init; } = null!;
    public Input<string> CloudflareAccountId { get; init; } = null!;
    public Input<string> GoogleProjectSlug { get; init; } = null!;
}
using Pulumi;

namespace App.Deploy.Component;

public class BackendComponentArgs
{
    public Input<string> Environment { get; init; } = null!;
    public Input<string> AuthDomain { get; init; } = null!;
    public Input<string> AuthAudience { get; init; } = null!;
    public Input<string> AuthScope { get; init; } = null!;
    public Input<string> NordigenSecretId { get; init; } = null!;
    public Input<string> NordigenSecretKey { get; init; } = null!;
    public Input<string> DatabaseConnectionString { get; init; } = null!;
    public Input<string> DatabaseName { get; init; } = null!;

}
using Pulumi;

namespace App.Deploy.Component;

public class DatabaseComponentArgs
{
    public Input<string> Environment { get; init; } = null!;
    public Input<string> MongoDbProjectId { get; init; } = null!;
    public Input<string> GoogleRegion { get; init; } = null!;
}
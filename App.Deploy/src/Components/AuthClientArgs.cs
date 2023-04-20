using System;
using Pulumi;

namespace App.Deploy.Components;

public class AuthClientArgs
{
    public InputList<string>? FrontendUrls { get; init; }
    public InputList<string>? CallbackUrls { get; init; }
    public Input<string> Environment { get; internal set; } = null!;
}
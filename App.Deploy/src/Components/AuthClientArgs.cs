using System;
using Pulumi;

namespace App.Deploy.Components;

public class AuthClientArgs
{
    public Input<string>? FrontendUrl { get; init; }
    public Input<string> Environment { get; internal set; } = null!;
}
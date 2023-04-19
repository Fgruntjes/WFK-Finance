using System.Collections.Generic;
using System.Collections.Immutable;
using Pulumi;

namespace App.Deploy.Components;

public class AuthComponentArgs
{
    public Input<string> Environment { get; init; } = null!;
    public IDictionary<string, string> Scopes { get; init; } = null!;
}
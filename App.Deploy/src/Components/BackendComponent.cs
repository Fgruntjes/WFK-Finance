using System.Collections.Generic;
using App.Deploy.Components;
using App.Deploy.Utils;
using Pulumi;

namespace App.Deploy.Component;

public class BackendComponent : ComponentResource
{
    public BackendComponent(
        string name,
        BackendComponentArgs args,
        ComponentResourceOptions opts
    ) : base("app:frontend", name, opts)
    {
        // So "npm run dev" use the deploy values
        EnvFileWriter.Write("../App.Backend/.local.env", new InputMap<string>() {
            {"AUTH0__DOMAIN", args.AuthDomain},
            {"AUTH0__AUDIENCE", args.AuthAudience},
            {"AUTH0__SCOPE", args.AuthScope},
        });
        RegisterOutputs();
    }
}

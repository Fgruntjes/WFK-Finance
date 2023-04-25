using System;
using System.Collections.Generic;
using App.Deploy.Components;
using App.Deploy.Utils;
using Pulumi;

namespace App.Deploy.Component;

public class BackendComponent : ComponentResource
{
    public Output<string> BaseUrl { get; private set; }

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
            {"NORDIGEN__SECRET_ID", args.NordigenSecretId},
            {"NORDIGEN__SECRET_KEY", args.NordigenSecretKey},
        });

        BaseUrl = Output.Create("http://localhost:5000");
        RegisterOutputs();
    }
}

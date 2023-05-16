using System;
using System.Linq;
using Pulumi;
using Pulumi.Auth0;
using Pulumi.Auth0.Inputs;

namespace App.Deploy.Components;

class AuthComponent : ComponentResource
{
    public Output<string> PublicApiAudience { get; private set; } = null!;

    public AuthComponent(string name, AuthComponentArgs args, ComponentResourceOptions? options = null)
        : base("app:auth0", name, options)
    {
        var publicApi = new ResourceServer(
            $"{name}-public",
            new ResourceServerArgs
            {
                Identifier = args.Environment.Apply(environment => $"{environment}-public"),
                Scopes = GetApiScopes(args)
            },
            new CustomResourceOptions { Parent = this });

        PublicApiAudience = publicApi.Identifier;
        RegisterOutputs();
    }

    private InputList<ResourceServerScopeArgs> GetApiScopes(AuthComponentArgs args)
    {
        return args.Scopes
            .Select(kvp => new ResourceServerScopeArgs
            {
                Value = kvp.Key,
                Description = kvp.Value
            }
            ).ToList();
    }

    private InputList<RolePermissionArgs> GetRolePermissions(AuthComponentArgs args, ResourceServer resourceServer)
    {
        return args.Scopes
            .Select(kvp => new RolePermissionArgs
            {
                ResourceServerIdentifier = resourceServer.Identifier,
                Name = kvp.Key
            }
            ).ToList();
    }
}
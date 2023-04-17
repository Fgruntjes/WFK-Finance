using System.Linq;
using Pulumi;
using Pulumi.Auth0;
using Pulumi.Auth0.Inputs;

namespace App.Deploy.Components;

class Auth0Component : ComponentResource
{
    public Auth0Component(string name, Auth0ComponentArgs args, ComponentResourceOptions? options = null)
        : base("app:auth0", name, options)
    {
        var publicApi = new ResourceServer(
            $"{name}-public",
            new ResourceServerArgs
            {
                Identifier = args.Environment.Apply(environment => $"${environment}-public"),
                Scopes = GetApiScopes(args)
            },
            new CustomResourceOptions { Parent = this });

        new Role(
            $"{name}-user",
            new()
            {
                Description = "App user",
                Permissions = GetRolePermissions(args, publicApi)
            },
            new CustomResourceOptions { Parent = this });

        new Rule(
            $"{name}-tenant-claim",
            new()
            {
                Script = args.Environment.Apply(environment => $@"
					function (user, context, callback) {{
						if (context.clientName.startsWith('{environment}:')) {{
							context.accessToken['app/tenants'] = user?.app_metadata?.tenants || [];
						}}

						return callback(null, user, context);
				}}")
            },
            new CustomResourceOptions { Parent = this });

        RegisterOutputs();
    }

    private InputList<ResourceServerScopeArgs> GetApiScopes(Auth0ComponentArgs args)
    {
        return args.Scopes
            .Select(kvp => new ResourceServerScopeArgs
            {
                Value = kvp.Key,
                Description = kvp.Value
            }
            ).ToList();
    }

    private InputList<RolePermissionArgs> GetRolePermissions(Auth0ComponentArgs args, ResourceServer resourceServer)
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
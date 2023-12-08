using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DotNetEnv;

namespace App.Lib.Configuration;

public static class DataProtectionExtension
{
    public static IHostBuilder UseDataProtection(this IHostBuilder builder)
    {
        var options = builder.UseOptions<Options.DataProtectionOptions>(Options.DataProtectionOptions.Section);

        if (options.Disabled)
        {
            return builder
                .ConfigureServices((context, services) =>
                {
                    services.AddDataProtection();
                });
        }

        return builder
            .ConfigureServices((context, services) =>
            {
                var credentials = context.HostingEnvironment.IsDevelopment()
                    ? CreateDevelopmentCredentials()
                    : new DefaultAzureCredential();

                var storageUri = new Uri($"{options.StorageAccountUri}{options.StorageContainer}/{options.KeyBlobName}");
                var keyUri = new Uri($"{options.KeyVaultUri.TrimEnd('/')}/keys/{options.KeyName}");

                services
                    .AddDataProtection()
                    .PersistKeysToAzureBlobStorage(storageUri, credentials)
                    .ProtectKeysWithAzureKeyVault(keyUri, credentials);

                services.AddSingleton(sp => new BlobServiceClient(new Uri(options.StorageAccountUri), credentials));
                services
                    .AddHealthChecks()
                    .AddAzureBlobStorage(blobOptions =>
                    {
                        blobOptions.ContainerName = options.StorageContainer;
                    }, tags: new[] { AppHealthCheckExtension.ReadinessTag })
                    .AddAzureKeyVault(new Uri(options.KeyVaultUri), credentials, vaultOptions =>
                    {
                        vaultOptions.AddKey(options.KeyName);
                    }, tags: new[] { AppHealthCheckExtension.ReadinessTag });
            });
    }

    private static TokenCredential CreateDevelopmentCredentials()
    {
        var env = Env
            .NoEnvVars()
            .TraversePath()
            .Load(".deploy.env")
            .ToDictionary();

        return new ClientSecretCredential(env["ARM_TENANT_ID"], env["ARM_CLIENT_ID"], env["ARM_CLIENT_SECRET"]);
    }
}
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App.Lib.Configuration;

public static class DataProtectionExtension
{
    public static IHostBuilder UseDataProtection(this IHostBuilder builder)
    {
        var options = builder.UseOptions<Options.DataProtectionOptions>(Options.DataProtectionOptions.Section);
        return builder
            .ConfigureServices((context, services) =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                {
                    services.AddDataProtectionDevelopment();
                }
                else
                {
                    services.AddDataProtectionProduction(options);
                }
            });
    }

    private static void AddDataProtectionDevelopment(this IServiceCollection services)
    {
        services
            .AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(".keys"));
    }

    private static void AddDataProtectionProduction(
        this IServiceCollection services,
        Options.DataProtectionOptions options)
    {
        var credentials = new DefaultAzureCredential();
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
            .AddAzureKeyVault(keyUri, credentials, vaultOptions =>
            {
                vaultOptions.AddKey(options.KeyName);
            }, tags: new[] { AppHealthCheckExtension.ReadinessTag });
    }
}
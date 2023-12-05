using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.DataProtection;

namespace App.Backend.Startup;

public static class DataProtection
{
    public static void AddAppDataProtection(
        this IServiceCollection services,
        string keyVaultUri,
        string keyName,
        string storageAccountUri,
        string storageContainer,
        string blobName)
    {
        if (keyName == "dev-backend-data-protection")
        {
            services
                .AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(".keys"));
        }
        else
        {
            var credentials = new DefaultAzureCredential();
            var storageUri = new Uri($"{storageAccountUri}{storageContainer}/{blobName}");
            var keyUri = new Uri($"{keyVaultUri.TrimEnd('/')}/keys/{keyName}");

            services
                .AddDataProtection()
                .PersistKeysToAzureBlobStorage(storageUri, credentials)
                .ProtectKeysWithAzureKeyVault(keyUri, credentials);

            services.AddSingleton(sp => new BlobServiceClient(new Uri(storageAccountUri), credentials));
            services
                .AddHealthChecks()
                .AddAzureBlobStorage(options =>
                {
                    options.ContainerName = storageContainer;
                }, tags: new[] { "readiness" })
                .AddAzureKeyVault(keyUri, credentials, options =>
                {
                    options.AddKey(keyName);
                }, tags: new[] { "readiness" });
        }
    }
}
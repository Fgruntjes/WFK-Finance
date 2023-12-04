using Azure.Identity;
using Microsoft.AspNetCore.DataProtection;

namespace App.Backend.Startup;

public static class DataProtection
{
    public static void AddAppDataProtection(this IServiceCollection services, string keyVaultServiceUri, string keyName)
    {
        services.AddAppDataProtection(new Uri(keyVaultServiceUri), keyName);
    }

    public static void AddAppDataProtection(this IServiceCollection services, Uri keyVaultServiceUri, string keyName)
    {
        if (keyName == "dev-backend-data-protection")
        {
            services
                .AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(".keys"));
        }
        else
        {
            services
                .AddDataProtection()
                .ProtectKeysWithAzureKeyVault(keyVaultServiceUri, new DefaultAzureCredential());

            services
                .AddHealthChecks()
                .AddAzureKeyVault(keyVaultServiceUri, new DefaultAzureCredential(), options =>
                {
                    options.AddKey(keyName);
                });
        }
    }
}
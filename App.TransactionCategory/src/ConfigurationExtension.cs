using App.TransactionCategory.Interface;
using App.TransactionCategory.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App.Lib.Data;

public static class ConfigurationExtension
{
    public static IHostBuilder UseTransactionCategory(this IHostBuilder builder)
    {
        return builder.ConfigureServices((hostContext, services) =>
        {
            services.AddScoped<ITransactionCategoryService, TransactionCategoryService>();
        });
    }
}

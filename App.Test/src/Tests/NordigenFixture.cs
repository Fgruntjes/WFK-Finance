
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VMelnalksnis.NordigenDotNet;

namespace App.Test.Tests;

public class NordigenFixture : PlaywrightFixture, IAsyncLifetime
{
    private readonly INordigenClient _nordigenClient;

    public NordigenFixture(ILoggerProvider loggerProvider) : base(loggerProvider)
    {
        _nordigenClient = Services.GetRequiredService<INordigenClient>();
    }

    public new async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await foreach (var requisition in _nordigenClient.Requisitions.Get())
        {
            if (requisition.InstitutionId == "SANDBOXFINANCE_SFIN0000")
            {
                await _nordigenClient.Requisitions.Delete(requisition.Id);
            }
        }
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

using Microsoft.Extensions.Hosting;

namespace App.Lib.ServiceBus;

public class ApplicationIdleHostService : IHostedService
{
    private readonly ApplicationIdleService _idleService;

    public ApplicationIdleHostService(ApplicationIdleService idleService)
    {
        _idleService = idleService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _idleService.ResetExitTimer();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.Lib.ServiceBus;

public class ApplicationIdleService
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IOptions<ServiceBusOptions> _options;
    private readonly TimeSpan _exitDelay = TimeSpan.FromSeconds(30);
    private Timer? _exitTimer;
    private readonly ILogger<ApplicationIdleService> _logger;

    public ApplicationIdleService(
        IHostApplicationLifetime appLifetime,
        IOptions<ServiceBusOptions> options,
        ILoggerFactory loggerFactory)
    {
        _appLifetime = appLifetime;
        _options = options;
        _logger = loggerFactory.CreateLogger<ApplicationIdleService>();
    }

    public void ResetExitTimer()
    {
        if (_exitTimer == null)
        {
            _exitTimer = new Timer(
                _ =>
                {
                    if (!_options.Value.QuitWhenIdle)
                    {
                        _logger.LogInformation("No messages, quit when idle disabled");
                        return;
                    }

                    _logger.LogInformation("No messages, exiting application");
                    _appLifetime.StopApplication();
                },
                null,
                _exitDelay,
                TimeSpan.FromDays(7));
        }
        else
        {
            _exitTimer.Change(_exitDelay, TimeSpan.FromDays(7));
        }

        _logger.LogInformation(
            "Exit timer delay {ExitDelay}, expected exit at {ExitTime}",
            _exitDelay,
            DateTime.Now + _exitDelay);
    }
}
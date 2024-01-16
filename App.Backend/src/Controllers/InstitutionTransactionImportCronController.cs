using App.Backend.Mvc;
using App.Institution.Interface;
using App.Lib.Configuration.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace App.Backend.Controllers;

[ApiController]
[ApiGroup(typeof(InstitutionAccountListController))]
[Route(RouteBase)]
public class InstitutionTransactionImportCronController : ControllerBase
{
    public const string RouteBase = "/institutionaccounts/all/cron/import";

    public const string RouteName = nameof(InstitutionTransactionImportCronController);

    private readonly ITransactionImportQueueService _importService;
    private readonly IOptions<AppOptions> _options;
    private readonly ILogger<InstitutionTransactionImportCronController> _log;

    public InstitutionTransactionImportCronController(
        ITransactionImportQueueService importService,
        IOptions<AppOptions> options,
        ILogger<InstitutionTransactionImportCronController> log)
    {
        _importService = importService;
        _options = options;
        _log = log;
    }

    [HttpGet(Name = RouteName)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Queue([FromQuery] string cronToken, CancellationToken cancellationToken = default)
    {
        if (cronToken != _options.Value.CronToken)
        {
            _log.LogCritical("Invalid cron token, make sure App.CronToken is set correctly in configuration");
            return NotFound();
        }

        await _importService.QueueAllAccountsAsync(cancellationToken);
        return Accepted();
    }
}

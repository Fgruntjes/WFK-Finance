using App.Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[Tags("BankConnection")]
[Produces("application/json")]
[Authorize("bankacounts:read")]
[Route("/BankConnection")]
public class BankConnectionController : ControllerBase
{
    private readonly BankConnectionService _bankConnectService;

    public BankConnectionController(BankConnectionService bankConnectService)
    {
        _bankConnectService = bankConnectService;
    }

    [HttpGet()]
    public async Task<BankConnection[]> List(int start = 0, int end = 0, CancellationToken cancellationToken = default)
    {
        return Array.Empty<BankConnection>();
    }
}

using App.Backend.DTO;
using App.Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[Tags("BankConnection")]
[Produces("application/json")]
[Authorize("bankacounts:read")]
[Route("/BankConnection")]
public class InstitutionConnectionController : ControllerBase
{
    private readonly BankConnectionService _bankConnectService;

    public InstitutionConnectionController(BankConnectionService bankConnectService)
    {
        _bankConnectService = bankConnectService;
    }

    [HttpGet()]
    public async Task<InstitutionConnection[]> List(int start = 0, int end = 0, CancellationToken cancellationToken = default)
    {
        return Array.Empty<InstitutionConnection>();
    }
}

using App.Backend.DTO;
using App.Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[Tags("InstitutionConnection")]
[Produces("application/json")]
[Authorize("bankacounts:read")]
[Route("/InstitutionConnection")]
public class InstitutionConnectionController : ControllerBase
{
    private readonly InstitutionConnectionService _bankConnectService;

    public InstitutionConnectionController(InstitutionConnectionService bankConnectService)
    {
        _bankConnectService = bankConnectService;
    }

    [HttpGet()]
    public async Task<InstitutionConnection[]> List(int start = 0, int end = 0, CancellationToken cancellationToken = default)
    {
        return Array.Empty<InstitutionConnection>();
    }
}

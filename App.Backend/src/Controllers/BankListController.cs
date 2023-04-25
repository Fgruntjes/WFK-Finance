using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMelnalksnis.NordigenDotNet;

namespace App.Backend.Controllers;

[ApiController]
[Route("Bank")]
[Tags("Bank accounts")]
[Produces("application/json")]
public class BankListController : ControllerBase
{
    private INordigenClient _nordigenClient;

    public BankListController(INordigenClient nordigenClient)
    {
        _nordigenClient = nordigenClient;
    }

    [HttpGet(Name = "GetBanks")]
    [Authorize("bankacounts:read")]
    public async IAsyncEnumerable<Bank> Get(string countryCode, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var banks = await _nordigenClient.Institutions.GetByCountry(countryCode, ct);
        foreach (var bank in banks)
        {
            yield return new Bank
            {
                Id = bank.Id,
                Name = bank.Name,
                Logo = bank.Logo,
            };
        }
    }
}

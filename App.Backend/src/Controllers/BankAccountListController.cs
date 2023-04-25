using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[ApiController]
[Route("BankAccount")]
[Tags("Bank accounts")]
[Produces("application/json")]
public class BankAccountListController : ControllerBase
{
    [HttpGet(Name = "GetBankAccounts")]
    [Authorize("bankacounts:read")]
    public IEnumerable<BankAccount> Get()
    {
        return Array.Empty<BankAccount>();
    }
}

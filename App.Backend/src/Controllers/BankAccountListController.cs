using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[ApiController]
[Route("Bank/Account")]
[Tags("Bank accounts")]
[Produces("application/json")]
public class BankAccountListController : ControllerBase
{
    [HttpGet(Name = "BankAccountList")]
    [Authorize("bankacounts:read")]
    public IEnumerable<BankAccount> Get()
    {
        return Array.Empty<BankAccount>();
    }
}

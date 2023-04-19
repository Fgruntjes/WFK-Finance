using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[ApiController]
[Route("BankAccount")]
public class BankAccountListController : ControllerBase
{
    private readonly ILogger<BankAccountListController> _logger;

    public BankAccountListController(ILogger<BankAccountListController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetBankAccounts")]
    [Authorize("bankacounts:read")]
    public IEnumerable<BankAccount> Get()
    {
        return Array.Empty<BankAccount>();
    }
}

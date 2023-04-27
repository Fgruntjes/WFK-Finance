using System.ComponentModel;
using App.Backend.Authorization;
using App.Backend.DTO;
using App.Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Backend.Controllers;

[ApiController]
[Route("Bank/Connect")]
[Tags("Bank accounts")]
[Produces("application/json")]
public class BankConnectController : ControllerBase
{
    private readonly INordigenClient _nordigenClient;
    private readonly BankConnectService _bankConnectService;

    public BankConnectController(INordigenClient nordigenClient, BankConnectService bankConnectService)
    {
        _nordigenClient = nordigenClient;
        _bankConnectService = bankConnectService;
    }

    public AuthContext AuthContext { get; }

    [HttpGet(Name = "BankConnect")]
    [Description("Connect to bank")]
    [Authorize("bankacounts:connect")]
    public async Task<BankConnectResponse> Connect(string bankId, string returnUrl, CancellationToken ct = default)
    {
        var requisitionResponse = await _nordigenClient.Requisitions.Post(new RequisitionCreation(new Uri(returnUrl), bankId)
        {
            AccountSelection = true
        });

        await _bankConnectService.StoreConnectUrl(bankId, requisitionResponse.Link, ct);

        return new BankConnectResponse
        {
            Url = requisitionResponse.Link
        };
    }
}

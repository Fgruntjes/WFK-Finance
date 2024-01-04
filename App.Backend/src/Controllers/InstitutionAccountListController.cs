using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend.Controllers;

[ApiController]
[Authorize]
[Route(RouteBase)]
public class InstitutionAccountListController : ControllerBase
{
    public const string RouteBase = "/institutionaccounts";
}

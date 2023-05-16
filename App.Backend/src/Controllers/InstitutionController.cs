using App.Backend.DTO;
using App.Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VMelnalksnis.NordigenDotNet;

namespace App.Backend.Controllers;

[Tags("Institution")]
[Produces("application/json")]
[Authorize("bankacounts:read")]
[Route("/Institution")]
public class InstitutionController : ControllerBase
{
    private readonly INordigenClient _nordigenClient;

    public InstitutionController(INordigenClient nordigenClient)
    {
        _nordigenClient = nordigenClient;
    }

    [HttpGet()]
    public async Task<Institution[]> List(string country, CancellationToken cancellationToken = default)
    {
        var institutions = await _nordigenClient.Institutions.GetByCountry(country);
        return institutions.Select(i => new Institution
        {
            Id = i.Id,
            Name = i.Name,
            Logo = i.Logo
        }).ToArray();
    }
}

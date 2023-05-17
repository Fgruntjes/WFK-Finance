using App.Backend.DTO;
using App.Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace App.Backend.Controllers;

[Tags("Institution")]
[Produces("application/json")]
[Authorize("bankacounts:read")]
[Route("/Institution")]
public class InstitutionController : ControllerBase
{
    private readonly InstitutionService _institutionService;

    public InstitutionController(InstitutionService institutionService)
    {
        _institutionService = institutionService;
    }

    [HttpGet()]
    [Route("/Institution/{id}")]
    public async Task<Institution> Get(string id, CancellationToken cancellationToken = default)
    {
        var institution = await _institutionService.Get(new ObjectId(id), cancellationToken);
        return Institution.FromEntity(institution);
    }

    [HttpGet()]
    [Route("/Institution")]
    public async Task<Institution[]> GetMany(IList<string> id, CancellationToken cancellationToken = default)
    {
        var institution = await _institutionService.GetMany(id.Select(id => new ObjectId(id)).ToList(), cancellationToken);
        return institution.Select(Institution.FromEntity).ToArray();
    }

    [HttpGet()]
    [Route("/Institution/Search")]
    public async Task<Institution[]> Search(string country, CancellationToken cancellationToken = default)
    {
        var institutions = await _institutionService.Search(country);
        return institutions.Select(i => new Institution
        {
            Id = i.Id.ToString(),
            Name = i.Name,
            Logo = i.Logo
        }).ToArray();
    }
}

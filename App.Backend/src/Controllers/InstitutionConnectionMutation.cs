using App.Backend.Data;
using App.Backend.GraphQL.Type;
using App.Backend.Service;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace App.Backend.Controllers;

public class InstitutionConnectionMutation : GraphController
{
    private readonly DatabaseContext _database;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly InstitutionConnectionCreateService _createService;

    public InstitutionConnectionMutation(
        DatabaseContext database,
        IHttpContextAccessor httpContextAccessor,
        InstitutionConnectionCreateService createService)
    {
        _database = database;
        _httpContextAccessor = httpContextAccessor;
        _createService = createService;
    }

    [Authorize]
    [MutationRoot("createInstitutionConnection")]
    public async Task<InstitutionConnection?> Create(Guid institutionId, Uri returnUrl, CancellationToken cancellationToken = default)
    {
        var entity = await _createService.Connect(institutionId, returnUrl, cancellationToken);
        return InstitutionConnection.FromEntity(entity);
    }
}

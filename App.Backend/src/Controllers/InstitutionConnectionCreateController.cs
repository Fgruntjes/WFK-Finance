using App.Backend.GraphQL.Controllers;
using App.Backend.GraphQL.Type;
using App.Lib.InstitutionConnection.Exception;
using App.Lib.InstitutionConnection.Service;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace App.Backend.Controllers;

[GraphRoute("institutionConnection")]
public class InstitutionConnectionCreateController : GraphController
{
    private readonly IInstitutionConnectionCreateService _createService;

    public InstitutionConnectionCreateController(IInstitutionConnectionCreateService createService)
    {
        _createService = createService;
    }

    [Authorize]
    [Mutation("create", typeof(InstitutionConnection))]
    public async Task<IGraphActionResult> Create(
        Guid institutionId,
        [FromGraphQL(TypeExpression = "Type!")] Uri returnUrl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _createService.Connect(institutionId, returnUrl, cancellationToken);
            return Ok(entity.ToGraphQLType());
        }
        catch (InstitutionNotFoundException exception)
        {
            return new BadRequestGraphQlActionResult(exception);
        }
    }
}
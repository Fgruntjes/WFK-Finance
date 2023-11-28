using App.Backend.GraphQL.Type;
using App.Backend.Service;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace App.Backend.Controllers;

[GraphRoute("institutionConnection")]
public class InstitutionConnectionCreateController : GraphController
{
    private readonly InstitutionConnectionCreateService _createService;

    public InstitutionConnectionCreateController(InstitutionConnectionCreateService createService)
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
            var entity = await _createService.Connect(User, institutionId, returnUrl, cancellationToken);
            return Ok(entity.ToGraphQLType());
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
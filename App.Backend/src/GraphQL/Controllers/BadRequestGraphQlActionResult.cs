using GraphQL.AspNet;

namespace App.Backend.GraphQL.Controllers;

internal class BadRequestGraphQlActionResult : ErrorGraphQlActionResult
{
    public BadRequestGraphQlActionResult(Exception exception)
        : base(exception, Constants.ErrorCodes.BAD_REQUEST)
    {
    }
}
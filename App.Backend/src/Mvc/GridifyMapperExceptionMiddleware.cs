using System.Text.RegularExpressions;
using Gridify;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace App.Backend.Mvc;

internal partial class GridifyMapperExceptionFilter : IExceptionFilter
{
    [GeneratedRegex("^(Mapping|Property) '([a-zA-Z0-9]+)' not found\\.?$")]
    private static partial Regex PropertyRegex();

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not GridifyMapperException)
        {
            return;
        }

        var matches = PropertyRegex().Matches(context.Exception.Message);
        if (matches.Count == 0)
        {
            return;
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = "An error occurred",
            Detail = "Property {Property} is not found",
            Status = (int)StatusCodes.Status400BadRequest,
            Extensions =
            {
                ["Property"] = matches[0].Groups[2].Value
            }
        });
        context.ExceptionHandled = true;
    }
}
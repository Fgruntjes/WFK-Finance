using App.Lib.Data.Exception;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace App.Backend.Mvc;

internal partial class UniqueConstraintExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not UniqueConstraintException)
        {
            return;
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = "An error occurred",
            Detail = "Request conflicted with existing entities.",
            Status = StatusCodes.Status409Conflict,
        });
        context.ExceptionHandled = true;
    }
}
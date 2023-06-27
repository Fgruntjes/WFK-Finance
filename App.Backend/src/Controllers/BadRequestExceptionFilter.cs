using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace App.Backend.Controllers;

public class BadRequestExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is ArgumentException exception)
        {
            context.Result = new BadRequestObjectResult(new
            {
                Message = exception.Message,
                ParamName = exception.ParamName,
            });

            context.ExceptionHandled = true;
        }
    }
}
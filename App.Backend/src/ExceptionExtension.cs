using System.Collections;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace App.Backend;

public static class ExceptionExtension
{
    public static ProblemDetails ToProblemDetails(
        this System.Exception exception,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred",
            Detail = exception.Message,
            Status = (int)statusCode,
        };

        foreach (DictionaryEntry entry in exception.Data)
        {
            problemDetails.Extensions[entry.Key.ToString() ?? string.Empty] = entry.Value?.ToString();
        }

        return problemDetails;
    }
}
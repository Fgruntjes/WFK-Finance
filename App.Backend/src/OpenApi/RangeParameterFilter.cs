using App.Backend.Dto;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Backend.OpenApi;

public class RangeParameterFilter : IOperationFilter, IParameterFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!operation.Responses.TryGetValue("200", out OpenApiResponse? responseValue))
            return;

        if (operation.Parameters.All(p => p.Name != "range"))
            return;

        var removeParameters = operation.Parameters
            .Where(p => p.Name is "End" or "Limit" or "Offset")
            .ToList();
        foreach (var parameter in removeParameters)
        {
            operation.Parameters.Remove(parameter);
        }

        var resource = operation.Tags.First()?.Name?.ToLower() ?? "resource";
        responseValue.Headers.Add("Content-Range", new OpenApiHeader
        {
            Description = $"Range of the items returned `{resource} <start>-<end>/<total>`",
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        });
    }

    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        if (context.ParameterInfo.ParameterType != typeof(RangeParameter))
            return;

        if (parameter.Name != "Start")
            return;

        parameter.Name = "range";
        parameter.Description = "Range of the items to return `[<start>,<end>]`";
        parameter.Schema = new OpenApiSchema
        {
            Type = "int[]"
        };
    }
}
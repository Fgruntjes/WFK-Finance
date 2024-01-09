using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Backend.OpenApi;

public class RangeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!operation.Responses.TryGetValue("200", out OpenApiResponse? responseValue))
            return;

        if (operation.Parameters.All(p => p.Name != "range"))
            return;

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
}

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Backend.OpenApi;

public class DtoSuffixFilterFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Type != "object")
        {
            return;
        }

        var typeName = context.Type.Name;
        if (!typeName.EndsWith("Dto"))
        {
            return;
        }

        schema.Title = typeName[..^3];
    }
}

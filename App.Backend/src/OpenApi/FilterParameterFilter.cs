using App.Backend.Dto;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Backend.OpenApi;

public class FilterParameterFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(FilterParameterValue))
        {
            schema.Type = "object";
            schema.Format = null;
            schema.Properties = new Dictionary<string, OpenApiSchema>();
            schema.AdditionalPropertiesAllowed = true;
            schema.AdditionalProperties = new OpenApiSchema
            {
                OneOf = new List<OpenApiSchema>
                {
                    new() { Type = "string" },
                    new() { Type = "integer", Format = "int32" },
                    new() { Type = "array", Items = new OpenApiSchema { Type = "string" } },
                    new() { Type = "array", Items = new OpenApiSchema { Type = "integer", Format = "int32" } }
                }
            };
        }
    }
}
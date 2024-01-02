using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Backend.OpenApi;

internal class RequireNonNullableFilter : ISchemaFilter
{
    private static NullabilityInfoContext _nullabilityContext = new();

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var typeProperties = context.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in schema.Properties)
        {
            if (IsSourceTypePropertyNullable(typeProperties, property.Key))
            {
                continue;
            }

            if (!schema.Required.Contains(property.Key))
            {
                schema.Required.Add(property.Key);
            }
        }
    }

    private static bool IsNullable(PropertyInfo propertyInfo)
    {
        if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null)
        {
            return true;
        }

        var nullabilityInfo = _nullabilityContext.Create(propertyInfo);
        return nullabilityInfo.ReadState == NullabilityState.Nullable;
    }

    private static bool IsSourceTypePropertyNullable(PropertyInfo[] typeProperties, string propertyName)
    {
        return typeProperties.Any(info =>
            info.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase) && IsNullable(info));
    }
}
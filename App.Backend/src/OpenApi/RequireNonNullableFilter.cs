using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Backend.OpenApi;

internal class NonNullableFilter : ISchemaFilter
{
    private static NullabilityInfoContext _nullabilityContext = new();

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Type == null)
            return;

        if (schema.Type != "object")
        {
            EnsureNonNullable(schema, context);
        }
        else
        {
            EnsureRequiredProperties(schema, context);
        }
    }

    private static void EnsureRequiredProperties(OpenApiSchema schema, SchemaFilterContext context)
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

    private static void EnsureNonNullable(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!PropertyIsNullable(context))
        {
            schema.Nullable = false;
        }
    }

    private static bool PropertyIsNullable(SchemaFilterContext context)
    {
        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(context.Type))
        {
            return Nullable.GetUnderlyingType(context.Type) != null;
        }

        if (context.MemberInfo != null)
        {
            return IsNullable(context.MemberInfo);
        }

        if (context.ParameterInfo != null)
        {
            return IsNullable(context.ParameterInfo);
        }

        return true;
    }

    private static bool IsNullable(ParameterInfo parameterInfo)
    {
        return IsNullable(_nullabilityContext.Create(parameterInfo), parameterInfo.ParameterType);
    }

    private static bool IsNullable(MemberInfo memberInfo)
    {
        var targetObject = memberInfo.ReflectedType
            ?? throw new System.Exception("Property does not have a target object");
        var targetProperty = targetObject.GetProperty(memberInfo.Name)
            ?? throw new System.Exception("Property not found on target object");

        return IsNullable(targetProperty);
    }

    private static bool IsNullable(PropertyInfo propertyInfo)
    {
        return IsNullable(_nullabilityContext.Create(propertyInfo), propertyInfo.PropertyType);
    }

    private static bool IsNullable(NullabilityInfo nullabilityInfo, Type type)
    {
        if (Nullable.GetUnderlyingType(type) != null)
        {
            return true;
        }

        return nullabilityInfo.ReadState == NullabilityState.Nullable;
    }

    private static bool IsSourceTypePropertyNullable(PropertyInfo[] typeProperties, string propertyName)
    {
        return typeProperties.Any(info =>
            info.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase) && IsNullable(info));
    }
}
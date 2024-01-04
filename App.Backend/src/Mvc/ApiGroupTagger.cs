using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Backend.Mvc;

internal static partial class ApiGroupTagger
{
    [GeneratedRegex("[A-Z][a-z]*")]
    private static partial Regex ControllerTagRegex();

    public static IList<string> GetTags(ApiDescription description)
    {
        var groupAttribute = description.CustomAttributes()
            .Where(attribute => attribute.GetType() == typeof(ApiGroupAttribute))
            .Cast<ApiGroupAttribute>()
            .FirstOrDefault();
        if (groupAttribute != null)
        {
            return new[] { GetControllerTag(groupAttribute.Type) };
        }

        return new[] { GetControllerTag(description.ActionDescriptor) };
    }

    private static string GetControllerTag(Type controllerType)
    {
        var controllerName = controllerType.Name;
        if (controllerName.EndsWith("Controller"))
        {
            controllerName = controllerName[..^10];
        }
        return GetControllerTag(controllerName);
    }

    private static string GetControllerTag(ActionDescriptor descriptor)
    {
        var controllerName = descriptor.RouteValues["controller"]
            ?? throw new System.Exception("Can not determine controller name from route values");
        return GetControllerTag(controllerName);
    }

    private static string GetControllerTag(string controllerName)
    {
        var matches = ControllerTagRegex().Matches(controllerName);
        if (matches.Count <= 1)
        {
            throw new System.Exception("Can not determine controller name from route values");
        }

        var lastMatchIndex = matches[^1].Index;
        return controllerName[..lastMatchIndex];
    }
}
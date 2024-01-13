using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Backend.OpenApi;

public class LowerCaseParameterFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        if (parameter.Name.IsNullOrEmpty())
            return;

        parameter.Name = parameter.Name[0].ToString().ToLower() + parameter.Name[1..];
    }
}

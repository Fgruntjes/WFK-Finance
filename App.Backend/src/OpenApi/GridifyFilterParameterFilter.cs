using Gridify;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.Backend.OpenApi;

public class GridifyFilterParameterFilter : IParameterFilter
{
    private static string _filterDescription = string.Join('\n', new[] {
        "Conditional Operators",
        "| Name                  | Operator | Usage example          |",
        "|-----------------------|----------|------------------------|",
        "| Equal                 | `=`      | `\"FieldName = Value\"`  |",
        "| NotEqual              | `!=`     | `\"FieldName !=Value\"`  |",
        "| LessThan              | `<`      | `\"FieldName < Value\"`  |",
        "| GreaterThan           | `>`      | `\"FieldName > Value\"`  |",
        "| GreaterThanOrEqual    | `>=`     | `\"FieldName >=Value\"`  |",
        "| LessThanOrEqual       | `<=`     | `\"FieldName <=Value\"`  |",
        "| Contains - Like       | `=*`     | `\"FieldName =*Value\"`  |",
        "| NotContains - NotLike | `!*`     | `\"FieldName !*Value\"`  |",
        "| StartsWith            | `^`      | `\"FieldName ^ Value\"`  |",
        "| NotStartsWith         | `!^`     | `\"FieldName !^ Value\"` |",
        "| EndsWith              | `$`      | `\"FieldName $ Value\"`  |",
        "| NotEndsWith           | `!$`     | `\"FieldName !$ Value\"` |",
        "",
        "Logical Operators",
        "| Name        | Operator            | Usage example                                                     |",
        "|-------------|---------------------|-------------------------------------------------------------------|",
        "| AND         | `,`                 | `\"FirstName = Value, LastName = Value2\"`                          |",
        "| OR          | <code>&#124;</code> | <code>\"FirstName=Value&#124;LastName=Value2\"</code>               |",
        "| Parenthesis | `()`                | <code>\"(FirstName=*Jo,Age<30)&#124;(FirstName!=Hn,Age>30)\"</code> |",
    });

    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        if (context.ParameterInfo.ParameterType != typeof(GridifyQuery))
        {
            return;
        }


        if (parameter.Name == "filter")
        {
            parameter.Description = _filterDescription;
        }
        else if (parameter.Name == "orderBy")
        {
            parameter.Description = "The ordering query expression can be built with a comma-delimited ordered list of field/property names followed by asc or desc keywords.";
            parameter.Example = new OpenApiString("FirstName asc, LastName desc");
        }
    }
}

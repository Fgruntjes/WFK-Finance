using System.Text.Json;
using System.Text.Json.Serialization;
using App.Backend.Json;

namespace App.Backend.Mvc;

public static class JsonOptions
{
    public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = {
            new RangeParameterJsonConverter(),
            new FilterParameterJsonConverter(),
            new SortParameterJsonConverter(),
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
        }
    };
}
using System.Text.Json;
using App.Backend.Json;

namespace App.Backend;

public static class JsonOptions
{
    public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions
    {
        Converters = { new RangeParameterJsonConverter() }
    };
}
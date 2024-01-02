using System.Text.Json;
using System.Text.Json.Serialization;
using App.Backend.Dto;

namespace App.Backend.Json;

public class RangeParameterJsonConverter : JsonConverter<RangeParameter>
{
    public override RangeParameter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Assuming the value is in the format "[from,to]"
        var rangeValues = reader.GetString()?.TrimStart('[').TrimEnd(']').Split(',')
            ?? throw new JsonException("Range must be in the format [from,to]");

        if (rangeValues.Length != 2 ||
            !int.TryParse(rangeValues[0], out var start) ||
            !int.TryParse(rangeValues[1], out var end))
        {
            throw new JsonException("Range must be in the format [from,to]");
        }

        return new RangeParameter(start, end);
    }

    public override void Write(Utf8JsonWriter writer, RangeParameter value, JsonSerializerOptions options)
    {
        writer.WriteStringValue($"[{value.Start},{value.End}]");
    }
}
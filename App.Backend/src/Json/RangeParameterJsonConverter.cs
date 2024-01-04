using System.Text.Json;
using System.Text.Json.Serialization;
using App.Backend.Dto;

namespace App.Backend.Json;

public class RangeParameterJsonConverter : JsonConverter<RangeParameter>
{
    public override RangeParameter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected start of array");
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.Number || !reader.TryGetInt32(out var start))
        {
            throw new JsonException("Expected start of range");
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.Number || !reader.TryGetInt32(out var end))
        {
            throw new JsonException("Expected end of range");
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.EndArray)
        {
            throw new JsonException("Expected end of array");
        }

        return new RangeParameter(start, end);
    }

    public override void Write(Utf8JsonWriter writer, RangeParameter value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Start);
        writer.WriteNumberValue(value.End);
        writer.WriteEndArray();
    }
}

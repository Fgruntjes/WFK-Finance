using System.Text.Json;
using System.Text.Json.Serialization;
using App.Backend.Dto;

namespace App.Backend.Json;

public class SortParameterJsonConverter : JsonConverter<SortParameter>
{
    public override SortParameter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected start of array");
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected property");
        }
        var property = reader.GetString()
            ?? throw new JsonException("Expected property");

        if (!reader.Read() || reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected direction");
        }
        var direction = reader.GetString()
            ?? throw new JsonException("Expected direction");

        if (!reader.Read() || reader.TokenType != JsonTokenType.EndArray)
        {
            throw new JsonException("Expected end of array");
        }

        if (direction.Equals("ASC", StringComparison.OrdinalIgnoreCase))
        {
            return new SortParameter(property, SortDirection.Asc);
        }
        else if (direction.Equals("DESC", StringComparison.OrdinalIgnoreCase))
        {
            return new SortParameter(property, SortDirection.Desc);
        }
        else
        {
            throw new JsonException("Expected direction to be 'asc' or 'desc'");
        }
    }

    public override void Write(Utf8JsonWriter writer, SortParameter value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteStringValue(value.Field);
        writer.WriteStringValue(value.Direction.ToString().ToUpperInvariant());
        writer.WriteEndArray();
    }
}

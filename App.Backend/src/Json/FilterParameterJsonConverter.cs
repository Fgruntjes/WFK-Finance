using System.Text.Json;
using System.Text.Json.Serialization;
using App.Backend.Dto;

namespace App.Backend.Json;

public class FilterParameterJsonConverter : JsonConverter<FilterParameter>
{
    public override FilterParameter? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var filterParameter = new FilterParameter();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return filterParameter;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString();
            reader.Read();

            var filterParameterValue = new FilterParameterValue();

            if (reader.TokenType == JsonTokenType.String)
            {
                filterParameterValue.StringValue = reader.GetString();
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                filterParameterValue.IntValue = reader.GetInt32();
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                if (reader.Read() && reader.TokenType == JsonTokenType.String)
                {
                    filterParameterValue.StringCollectionValue = new List<string>();
                    while (reader.TokenType != JsonTokenType.EndArray)
                    {
                        filterParameterValue.StringCollectionValue.Add(reader.GetString() ?? throw new JsonException());
                        reader.Read();
                    }
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    filterParameterValue.IntCollectionValue = new List<int>();
                    while (reader.TokenType != JsonTokenType.EndArray)
                    {
                        filterParameterValue.IntCollectionValue.Add(reader.GetInt32());
                        reader.Read();
                    }
                }
                else if (reader.TokenType != JsonTokenType.Null)
                {
                    throw new JsonException();
                }
            }
            else if (reader.TokenType != JsonTokenType.Null)
            {
                throw new JsonException();
            }

            filterParameter.Add(propertyName, filterParameterValue);
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, FilterParameter value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var property in value.Keys)
        {
            writer.WritePropertyName(property);
            var propertyValue = value[property];
            if (propertyValue.StringValue != null)
            {
                writer.WriteStringValue(propertyValue.StringValue);
            }
            else if (propertyValue.StringCollectionValue != null)
            {
                writer.WriteStartArray();
                foreach (var stringValue in propertyValue.StringCollectionValue)
                {
                    writer.WriteStringValue(stringValue);
                }
                writer.WriteEndArray();
            }
            else if (propertyValue.IntValue != null)
            {
                writer.WriteNumberValue(propertyValue.IntValue.Value);
            }
            else if (propertyValue.IntCollectionValue != null)
            {
                writer.WriteStartArray();
                foreach (var intValue in propertyValue.IntCollectionValue)
                {
                    writer.WriteNumberValue(intValue);
                }
                writer.WriteEndArray();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        writer.WriteEndObject();
    }
}
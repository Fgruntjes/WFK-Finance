using System.Text.Json;
using System.Text.Json.Serialization;
using App.Backend.Dto;

namespace App.Backend.Json;

public class FilterParameterJsonConverter : JsonConverter<FilterParameter>
{
    public override FilterParameter? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
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
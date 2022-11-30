// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Serialization;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Converters;

internal class OperationContextJsonConverter : JsonConverter<IOperationContext>
{
    private Dictionary<string, Type> _types;

    public OperationContextJsonConverter()
    {
        var type = typeof(IOperationContext);
        _types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
            .ToDictionary(t => t.FullName, t => t);
    }

    /// <summary>
    /// Method that populates the reader object with values from the Json string.
    /// </summary>
    /// <param name="reader">JsonReader</param>
    /// <param name="objectType">Holds any class type.</param>
    /// <param name="existingValue">contains reference of a control/object.</param>
    /// <param name="serializer">JsonSerializer</param>
    public override IOperationContext Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //var values = JsonSerializer.Deserialize<Dictionary<string, object>>(
        //    JsonDocument.ParseValue(ref reader), options);
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        IOperationContext result = null;
        using (var jsonDocument = JsonDocument.ParseValue(ref reader))
        {
            if (!jsonDocument.RootElement.TryGetProperty("$Type", out var typeProperty))
            {
                throw new JsonException();
            }

            if (!_types.TryGetValue(typeProperty.GetString(), out var type))
            {
                throw new JsonException($"Type not found \"{typeProperty}\"");
            }

            if (!jsonDocument.RootElement.TryGetProperty("Value", out var typeValue))
            {
                throw new JsonException();
            }

            //var jsonObject = jsonDocument.RootElement.GetRawText();
            result = (IOperationContext)JsonSerializer.Deserialize(typeValue.ToString(), type, options);
        }


        return result;
    }

    /// <summary>
    /// converts the serializes the specified object and converts it into Json object.
    /// </summary>
    /// <param name="writer">JsaonWriter.</param>
    /// <param name="value">contains reference of a BaseModel.</param>
    /// <param name="serializer">JsonSerializer</param>
    public override void Write(Utf8JsonWriter writer, IOperationContext value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("$Type", value.GetType().FullName);
        writer.WritePropertyName("Value");
        JsonSerializer.Serialize(writer, value, value.GetType());
        writer.WriteEndObject();
    }
}
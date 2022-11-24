﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Serialization;
using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Converters;

public class OperationContextJsonConverter : JsonConverter<IPayload>
{
    private Dictionary<string, Type> _types;

    public OperationContextJsonConverter()
    {
        var type = typeof(IPayload);
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
    public override IPayload Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //var values = JsonSerializer.Deserialize<Dictionary<string, object>>(
        //    JsonDocument.ParseValue(ref reader), options);
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        IPayload result = null;
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
            result = (IPayload)JsonSerializer.Deserialize(typeValue.ToString(), type, options);
        }


        return result;
    }

    /// <summary>
    /// converts the serializes the specified object and converts it into Json object.
    /// </summary>
    /// <param name="writer">JsaonWriter.</param>
    /// <param name="value">contains reference of a BaseModel.</param>
    /// <param name="serializer">JsonSerializer</param>
    public override void Write(Utf8JsonWriter writer, IPayload value, JsonSerializerOptions options)
    {
        //var values = new Dictionary<string, object>
        //{
        //    { nameof(value.RequestId), value.RequestId }
        //};
        //if(value.Payload != null)
        //{
        //    values[nameof(value.Payload)] = value.Payload;
        //    values[$"{nameof(value.Payload)}Type"] = value.Payload.GetType().FullName;
        //}

        writer.WriteStartObject();
        writer.WriteString("$Type", value.GetType().FullName);
        writer.WritePropertyName("Value");
        JsonSerializer.Serialize(writer, value, value.GetType());
        writer.WriteEndObject();

        //JsonSerializer.Serialize(writer, values, options);
    }
}
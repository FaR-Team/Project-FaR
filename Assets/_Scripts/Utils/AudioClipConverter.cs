using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class AudioClipConverter : JsonConverter<AudioClip>
{
    public override void WriteJson(JsonWriter writer, AudioClip value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("name");
        writer.WriteValue(value.name);
        writer.WritePropertyName("length");
        writer.WriteValue(value.length);
        writer.WriteEndObject();
    }

    public override AudioClip ReadJson(JsonReader reader, Type objectType, AudioClip existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var jo = JObject.Load(reader);
        // Note: We can't create AudioClips at runtime, so we'll just return null here
        return null;
    }
}

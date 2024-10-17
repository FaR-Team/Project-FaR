using System;
using Newtonsoft.Json;
using UnityEngine;

public class SpriteConverter : JsonConverter<Sprite>
{
    public override void WriteJson(JsonWriter writer, Sprite value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteValue(value.name);
        }
    }

    public override Sprite ReadJson(JsonReader reader, Type objectType, Sprite existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        string spriteName = (string)reader.Value;
        return Resources.Load<Sprite>(spriteName);
    }
}

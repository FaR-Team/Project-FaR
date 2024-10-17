using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class GameObjectConverter : JsonConverter<GameObject>
{
    public override void WriteJson(JsonWriter writer, GameObject value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("name");
        writer.WriteValue(value.name);
        writer.WritePropertyName("tag");
        writer.WriteValue(value.tag);
        writer.WritePropertyName("layer");
        writer.WriteValue(value.layer);
        writer.WriteEndObject();
    }

    public override GameObject ReadJson(JsonReader reader, Type objectType, GameObject existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var jo = JObject.Load(reader);
        var go = new GameObject();
        go.name = jo["name"].Value<string>();
        go.tag = jo["tag"].Value<string>();
        go.layer = jo["layer"].Value<int>();
        return go;
    }
}

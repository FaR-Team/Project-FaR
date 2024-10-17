using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class MeshConverter : JsonConverter<Mesh>
{
    public override void WriteJson(JsonWriter writer, Mesh value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("name");
        writer.WriteValue(value.name);
        writer.WritePropertyName("subMeshCount");
        writer.WriteValue(value.subMeshCount);
        writer.WriteEndObject();
    }

    public override Mesh ReadJson(JsonReader reader, Type objectType, Mesh existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jo = JObject.Load(reader);
        var mesh = new Mesh();
        mesh.name = jo["name"].Value<string>();
        return mesh;
    }
}

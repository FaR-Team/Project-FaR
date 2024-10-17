using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class InventoryItemDataConverter : JsonConverter<InventoryItemData>
{
    public override InventoryItemData ReadJson(JsonReader reader, Type objectType, InventoryItemData existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);
        
        InventoryItemData itemData = ScriptableObject.CreateInstance<InventoryItemData>();
        
        itemData.ID = jo["ID"].Value<int>();
        itemData.Nombre = jo["Nombre"].Value<string>();
        itemData.Descripción = jo["Descripción"].Value<string>();
        itemData.Valor = jo["Valor"].Value<int>();
        itemData.Usable = jo["Usable"].Value<bool>();
        itemData.Sellable = jo["Sellable"].Value<bool>();
        itemData.IsLookingAtStore = jo["IsLookingAtStore"].Value<bool>();
        itemData.typeOfItem = (TypeOfItem)jo["typeOfItem"].Value<int>();

        if (jo["Icono"] != null)
        {
            var iconoData = jo["Icono"];
            var texture = new Texture2D(
                iconoData["texture"]["width"].Value<int>(),
                iconoData["texture"]["height"].Value<int>()
            );
            var rect = new Rect(
                iconoData["textureRect"]["x"].Value<float>(),
                iconoData["textureRect"]["y"].Value<float>(),
                iconoData["textureRect"]["width"].Value<float>(),
                iconoData["textureRect"]["height"].Value<float>()
            );
            var pivot = new Vector2(
                iconoData["pivot"]["x"].Value<float>(),
                iconoData["pivot"]["y"].Value<float>()
            );
            itemData.Icono = Sprite.Create(texture, rect, pivot);
        }

        if (jo["ItemPrefab"] != null)
        {
            itemData.ItemPrefab = new GameObject(jo["ItemPrefab"]["name"].Value<string>());
        }

        if (jo["DirtPrefabGhost"] != null)
        {
            itemData.DirtPrefabGhost = new GameObject(jo["DirtPrefabGhost"]["name"].Value<string>());
        }

        if (jo["ghostMesh"] != null)
        {
            itemData.ghostMesh = new Mesh();
            itemData.ghostMesh.name = jo["ghostMesh"]["name"].Value<string>();
        }

        return itemData;
    }

    public override void WriteJson(JsonWriter writer, InventoryItemData value, JsonSerializer serializer)
    {
        JObject jo = new JObject();

        jo["ID"] = value.ID;
        jo["Nombre"] = value.Nombre;
        jo["Descripción"] = value.Descripción;
        jo["Valor"] = value.Valor;
        jo["Usable"] = value.Usable;
        jo["Sellable"] = value.Sellable;
        jo["IsLookingAtStore"] = value.IsLookingAtStore;
        jo["typeOfItem"] = (int)value.typeOfItem;

        if (value.Icono != null)
        {
            jo["Icono"] = new JObject
            {
                ["textureName"] = value.Icono.texture.name,
                ["textureRect"] = new JObject
                {
                    ["x"] = value.Icono.textureRect.x,
                    ["y"] = value.Icono.textureRect.y,
                    ["width"] = value.Icono.textureRect.width,
                    ["height"] = value.Icono.textureRect.height
                },
                ["pivot"] = new JObject
                {
                    ["x"] = value.Icono.pivot.x,
                    ["y"] = value.Icono.pivot.y
                }
            };
        }

        jo["ItemPrefab"] = JToken.FromObject(value.ItemPrefab, serializer);
        jo["DirtPrefabGhost"] = JToken.FromObject(value.DirtPrefabGhost, serializer);
        jo["ghostMesh"] = JToken.FromObject(value.ghostMesh, serializer);
        jo["ToolGameObject"] = JToken.FromObject(value.ToolGameObject, serializer);
        jo["useItemSound"] = JToken.FromObject(value.useItemSound, serializer);

        jo.WriteTo(writer);
    }
}
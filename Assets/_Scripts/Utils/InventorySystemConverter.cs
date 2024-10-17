using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class InventorySystemConverter : JsonConverter<InventorySystem>
{
    public override void WriteJson(JsonWriter writer, InventorySystem value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("inventorySlots");
        serializer.Serialize(writer, value.InventorySlots);
        writer.WritePropertyName("gold");
        writer.WriteValue(value.Gold);
        writer.WritePropertyName("hotbarAbilitySlots");
        serializer.Serialize(writer, value.hotbarAbilitySlots);
        writer.WriteEndObject();
    }
    public override InventorySystem ReadJson(JsonReader reader, Type objectType, InventorySystem existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        try
        {
            JObject jsonObject = JObject.Load(reader);
            InventorySystem inventorySystem = new InventorySystem(0); // Assuming InventorySystem constructor takes an initial gold value
        
            inventorySystem.inventorySlots = jsonObject["inventorySlots"].ToObject<List<InventorySlot>>(serializer);
            inventorySystem._gold = jsonObject["gold"].Value<int>();
            inventorySystem.hotbarAbilitySlots = jsonObject["hotbarAbilitySlots"].ToObject<List<InventorySlot>>(serializer);

            return inventorySystem;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in InventorySystemConverter: {ex.Message}");
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

public static class InventoryLoader
{
    public static DummyLogger logger;

    static InventoryLoader()
    {
        logger = GameObject.FindObjectOfType<DummyLogger>();
        if (logger == null)
        {
            GameObject loggerObject = new GameObject("DataSaverLogger");
            logger = loggerObject.AddComponent<DummyLogger>();
            UnityEngine.Object.DontDestroyOnLoad(loggerObject);
        }
    }

    public static InventoryData inventoryData;
    /*
     PODEMOS HACER QUE ESTOS OBJETOS SIEMPRE ESTEN, AUNQUE SEA VACIOS SE ENCONTRARAN CADA VEZ QUE SE CREA UNA NUEVA RUN.
     HACIENDO QUE ESTE TRY SEA INUTIL.
     */
    public static InventorySystem Load(int defaultSize, int defaultGold, bool temporary)
    {
        logger.Log($"Attempting to load inventory. Temporary: {temporary}");
        try
        {
            string json = LoadAllData.GetRawData();
            logger.Log($"Loaded InventoryData: {json}");

            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> 
                {
                    new InventorySystemConverter(),
                    new SpriteConverter()
                },
                TypeNameHandling = TypeNameHandling.Auto,
                Error = HandleDeserializationError
            };

            inventoryData = JsonConvert.DeserializeObject<InventoryData>(json, settings);
        
            if (inventoryData != null && inventoryData.inventorySystem != null)
            {
                var loadedInventory = inventoryData.inventorySystem;
                logger.Log($"Loaded InventorySystem: {JsonConvert.SerializeObject(loadedInventory)}");
            
                // Log the contents of the inventory
                for (int i = 0; i < loadedInventory.InventorySlots.Count; i++)
                {
                    var slot = loadedInventory.InventorySlots[i];
                    logger.Log($"Slot {i}: ItemData: {JsonConvert.SerializeObject(slot.ItemData)}, StackSize: {slot.StackSize}");
                }
            
                logger.LogSuccess("Inventory loaded and populated successfully", loadedInventory.InventorySlots.Count + " slots");
                return loadedInventory;
            }
            else
            {
                logger.LogWarning("Inventory data or inventory system is null. Creating a new inventory.");
                return CreateDefaultInventory(defaultSize, defaultGold);
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to load Inventory information. Reason: {e}");
            return CreateDefaultInventory(defaultSize, defaultGold);
        }
    }

    private static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
    {
        var currentError = errorArgs.ErrorContext.Error.Message;
        logger.LogWarning($"Error deserializing: {currentError}");
        errorArgs.ErrorContext.Handled = true;
    }

    private static InventorySystem CreateDefaultInventory(int defaultSize, int defaultGold)
    {
        return new InventorySystem(defaultSize, defaultGold);
    }
}

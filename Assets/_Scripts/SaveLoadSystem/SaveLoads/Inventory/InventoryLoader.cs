using System;
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
            inventoryData = LoadAllData.GetData<InventoryData>(temporary);
            
            if (inventoryData != null && inventoryData.inventorySystem != null)
            {
                return inventoryData.inventorySystem;
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

    private static InventorySystem CreateDefaultInventory(int defaultSize, int defaultGold)
    {
        return new InventorySystem(defaultSize, defaultGold);
    }
}

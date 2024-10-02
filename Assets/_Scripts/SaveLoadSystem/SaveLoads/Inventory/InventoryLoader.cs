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
    public static InventorySystem Load(int defaultSize, int defaultgold)
    {
        return TryPreloadSavedDirts(defaultSize, defaultgold);
    }
    private static InventorySystem TryPreloadSavedDirts(int defaultSize, int defaultgold)
    {
        try
        {
            inventoryData = LoadAllData.GetData<InventoryData>();

            return inventoryData.inventorySystem;
        }
        catch (Exception e)
        {
            logger.LogWarning($"Failed to preload Inventory information. reason {e}");

            return new InventorySystem(defaultSize, defaultgold);
        }
    }
}

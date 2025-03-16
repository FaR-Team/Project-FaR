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
    public static InventorySystem Load(int defaultSize, int defaultgold, bool temporary)
    {
        return TryGetInventoryData(defaultSize, defaultgold, temporary);
    }
    private static InventorySystem TryGetInventoryData(int defaultSize, int defaultgold, bool temporary) // TODO: Para el player no va a haber temporal probablemente, ya que no se destruye
    {
        try
        {
            inventoryData = LoadAllData.GetData<InventoryData>(temporary);

            return inventoryData.inventorySystem;
        }
        catch (Exception e)
        {
            logger.LogWarning($"Failed to preload Inventory information. reason {e}");

            return new InventorySystem(defaultSize, defaultgold);
        }
    }
}

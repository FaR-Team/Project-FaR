using System;
using UnityEngine;

public static class InventoryLoader
{
    public static InventoryData inventoryData;
    /*
     PODEMOS HACER QUE ESTOS OBJETOS SIEMPRE ESTEN, AUQNEU SEA VACIOS SE ENCONTRARAN CADA VEZ QUE SE CREA UNA NUEVA RUN.
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
            // Se hardcodea false, pero esto debe ser dado por un game status manager,
            // que le diga si viene por primera vez o bien, si vuelve de algun sitio.
            inventoryData = LoadAllData.GetData<InventoryData>(false);
           
            return inventoryData.inventorySystem;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to preload Inventory information. reason {e}");
            
            return new InventorySystem(defaultSize, defaultgold);
        }
    }
}

using UnityEngine;

[System.Serializable]
public class ChestData : SaveData
{
    public string id;
    public InventorySystem inventorySystem;

    public ChestData(string id, InventorySystem inventorySystem)
    {
        this.id = id;
        this.inventorySystem = inventorySystem;
    }
}

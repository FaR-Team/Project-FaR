using UnityEngine;

[System.Serializable]
public class ChestData : SaveData
{
    public Vector3 position;
    public InventorySystem inventorySystem;

    public ChestData(Vector3 position, InventorySystem inventorySystem)
    {
        this.position = position;
        this.inventorySystem = inventorySystem;
    }
}

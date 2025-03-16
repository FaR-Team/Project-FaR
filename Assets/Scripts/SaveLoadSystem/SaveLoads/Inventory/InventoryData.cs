using System;
using UnityEngine;

[Serializable]
public class InventoryData : SaveData, IAllData<InventoryData>
{
    public InventorySystem inventorySystem;
    public void CopyData(InventoryData allData)
    {
        inventorySystem = allData.inventorySystem;
    }
    public InventoryData(InventorySystem inventorySystem)
    {
        this.inventorySystem = inventorySystem;
    }
    public InventoryData()
    {
        this.inventorySystem = null;
    }
}

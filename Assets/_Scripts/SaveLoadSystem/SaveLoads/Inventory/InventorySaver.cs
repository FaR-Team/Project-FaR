using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

public class InventorySaver : Saver<InventoryData, InventoryDataSaver>
{
    public static InventorySaver Instance;
    private InventoryData InventoryData;
    private InventoryDataSaver inventoryDataSaver;

    private void Awake()
    {
        Instance = this;
    }
    public override void AddSavedObject(InventoryDataSaver y)
    {
        inventoryDataSaver = y;
    }

    public override void RemoveSavedObject(InventoryDataSaver y)
    {
        inventoryDataSaver = null;
    }

    public override Task WriteSave(InventoryData t)
    {
        InventoryData = t;

        return Task.CompletedTask;
    }

    protected override async void SaveAllData(bool isTemporarySave)
    {
        try
        {
            await SaveInventory();

            SaverManager.Save(InventoryData, isTemporarySave);

            this.Log("Successfully Saved Inventory");
        }
        catch (Exception e)
        {
            this.LogError($"Failed Save Inventory. Reason: {e}");
        }
    }

    private async Task SaveInventory()
    {
        await inventoryDataSaver.SaveData();
    }
}

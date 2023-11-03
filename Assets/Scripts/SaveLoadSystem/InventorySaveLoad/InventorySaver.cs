using System.Threading.Tasks;

public class InventorySaver : Saver<InventorySlot, InventorySlotSave>
{
    public static InventorySaver Instance;

    private void Awake()
    {
        Instance = this;
    }
    protected override void SaveAllData(bool isTemporarySave)
    {
        throw new System.NotImplementedException();
    }

    public override Task WriteSave(InventorySlot t)
    {
        foreach (var slot in DynamicInventoryDisplayBackpack.instance.inventorySlots)
        {
            slot.SaveSlot();
        }
        return Task.CompletedTask;
    }
    public override void AddSavedObject(InventorySlotSave y)
    {
        throw new System.NotImplementedException();
    }

    public override void RemoveSavedObject(InventorySlotSave y)
    {
        throw new System.NotImplementedException();
    }
}

public class InventorySlotSave
{
    public async Task SaveData()
    {
        DirtData dirtSaveData =
            new DirtData(dirt._isWet, dirt.IsEmpty, dirt.currentSeedData, dirt.GetCropSaveData(), transform.position);

        await DirtSaver.instance.WriteSave(dirtSaveData);
    }
}
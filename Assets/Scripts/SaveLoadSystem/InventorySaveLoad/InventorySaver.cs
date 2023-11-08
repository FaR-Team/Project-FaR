using System.Threading.Tasks;

public class InventorySaver : Saver<InventorySlot, SaveBackPackData>
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
    public override void AddSavedObject(SaveBackPackData y)
    {
        throw new System.NotImplementedException();
    }

    public override void RemoveSavedObject(SaveBackPackData y)
    {
        throw new System.NotImplementedException();
    }
}

public class SaveBackPackData
{
    public async Task SaveData()
    {
       // DirtData dirtSaveData =
         //   new DirtData(dirt._isWet, dirt.IsEmpty, dirt.currentSeedData, dirt.GetCropSaveData(), transform.position);

     //   await DirtSaver.instance.WriteSave(dirtSaveData);
    }
}
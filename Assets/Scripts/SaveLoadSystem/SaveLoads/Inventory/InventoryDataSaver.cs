using System.Threading.Tasks;

public class InventoryDataSaver : DataSaver<PlayerInventoryHolder, InventoryDataSaver>, IDataSavable
{
    public override async Task SaveData()
    {
        InventoryData invData = new InventoryData(PlayerInventoryHolder.instance.PrimaryInventorySystem);
        
        await saverAllData.WriteSave(invData);
    }

    protected override void SetThisInstance()
    {
        thisDataSaver = this;
        saverAllData = InventorySaver.Instance;
    }
}
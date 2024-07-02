using System.Threading.Tasks;
using UnityEngine;

public class InventoryDataSaver : DataSaver<PlayerInventoryHolder, InventoryDataSaver>, IDataSavable
{
    [SerializeField] InventorySaver saver;

    protected override void SetThisInstance()
    {
        thisDataSaver = this;
        saver = InventorySaver.Instance;
        saverAllData = saver;
    }
    public override async Task SaveData()
    {
        InventoryData invData = new(PlayerInventoryHolder.instance.PrimaryInventorySystem);

        await saverAllData.WriteSave(invData);
    }
}
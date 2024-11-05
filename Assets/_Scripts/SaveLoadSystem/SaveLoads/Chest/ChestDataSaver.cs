using UnityEngine;
using System.Threading.Tasks;

public class ChestDataSaver : DataSaver<Cofre, ChestDataSaver>, IDataSavable
{
    protected override void SetThisInstance()
    {
        thisDataSaver = this;
        saverAllData = ChestSaver.instance; 
    }

    public override async Task SaveData()
    {
        ChestData chestData = new ChestData(uniqueiD.ID, objectToSave.PrimaryInventorySystem);

        await saverAllData.WriteSave(chestData);
    }
}

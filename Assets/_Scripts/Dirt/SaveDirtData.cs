using UnityEngine;
using System.Threading.Tasks;

[RequireComponent(typeof(Dirt))]
public class SaveDirtData : DataSaver<Dirt, SaveDirtData>, IDataSavable
{
    protected override void SetThisInstance()
    {
        thisDataSaver = this;
        saverAllData = DirtSaver.instance;
    }   
    
    public override async Task SaveData()
    {
        DirtData dirtSaveData = 
            new DirtData(objectToSave._isWet, objectToSave.IsEmpty, objectToSave.currentSeedData.ID, objectToSave.GetCropSaveData(), transform.position);

        await saverAllData.WriteSave(dirtSaveData);
    }
}
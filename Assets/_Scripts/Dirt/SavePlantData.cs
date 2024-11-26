using UnityEngine;
using System.Threading.Tasks;

[RequireComponent(typeof(TreeGrowing))]
public class SavePlantData : DataSaver<TreeGrowing, SavePlantData>, IDataSavable
{
    protected override void SetThisInstance()
    {
        thisDataSaver = this;
        saverAllData = PlantSaver.instance;
    }   
    
    public override async Task SaveData()
    {
        PlantData dirtSaveData = 
            new PlantData( objectToSave.DaysPlanted, objectToSave.ReGrowCounter, objectToSave.DaysWithoutHarvest, objectToSave.DaysWithoutFruits, objectToSave.UsedSpawnPoints, objectToSave.currentState, transform.position);

        await saverAllData.WriteSave(dirtSaveData);
    }
}
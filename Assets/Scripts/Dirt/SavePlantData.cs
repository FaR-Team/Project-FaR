using System.Linq;
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
        TreeBushData dirtSaveData = 
            new TreeBushData(objectToSave);

        await saverAllData.WriteSave(dirtSaveData);
    }
}
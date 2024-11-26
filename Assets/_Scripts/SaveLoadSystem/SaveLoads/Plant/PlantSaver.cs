using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class PlantSaver : Saver<PlantData, SavePlantData>
{
    public static PlantSaver instance;

    private AllPlantsData allPlantsData = new AllPlantsData();

    private List<SavePlantData> plants = new List<SavePlantData>();

    private void Awake()
    {
        if(instance != null && instance != this) Destroy(this);
        else instance = this;
    }
    protected async override void SaveAllData(bool isTemporarySave)
    {
        try
        {
            //Debug.Log("Dirts to save: " + dirts.Count);
            //if (dirts.Count == 0) return; // Don't save if no dirts to save
            
            //Debug.Log("START SAVING DIRTS");
            await SaveDirts();

            allPlantsData.SaveQueue(SceneManager.GetActiveScene().name);
            SaverManager.Save(allPlantsData, isTemporarySave);
            allPlantsData.ClearAfterSave();

            this.LogSuccess("Successfully Saved dirts information in scene " + SceneManager.GetActiveScene().name );
        }
        catch (Exception e)
        {
            this.LogError("Failed Save Dirt. Reason: " + e);
        }
    }

    public override Task WriteSave(PlantData info)
    {
        allPlantsData.data.Enqueue(info);
        allPlantsData.counter++;
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Loads all previously saved Scene Datas in current AllDirtsData
    /// </summary>
    /// <param name="datas"></param>
    public void LoadScenesData(List<ScenePlantData> datas)
    {
        allPlantsData.SetScenesDataOnLoad(datas);
    }

    private async Task SaveDirts()
    {
        foreach (var dirt in plants)
        {
            await dirt.SaveData();
        }
    }

    public override void AddSavedObject(SavePlantData dirtData)
    {
        //Debug.Log("Added dirt to save in DirtSaver");
        plants.Add(dirtData);
    }

    public override void RemoveSavedObject(SavePlantData dirtData)
    {
        plants.Remove(dirtData);
    }
}

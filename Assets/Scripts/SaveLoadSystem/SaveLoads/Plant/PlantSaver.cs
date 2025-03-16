using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class PlantSaver : Saver<TreeBushData, SavePlantData>
{
    public static PlantSaver instance;

    private AllTreesData _allTreesData = new AllTreesData();

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

            _allTreesData.SaveQueue(SceneManager.GetActiveScene().name);
            SaverManager.Save(_allTreesData, isTemporarySave);
            _allTreesData.ClearAfterSave();

            //this.LogSuccess("Successfully Saved dirts information in scene " + SceneManager.GetActiveScene().name );
        }
        catch (Exception e)
        {
            this.LogError("Failed Save Dirt. Reason: " + e);
        }
    }

    public override Task WriteSave(TreeBushData info)
    {
        _allTreesData.data.Enqueue(info);
        _allTreesData.counter++;
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Loads all previously saved Scene Datas in current AllDirtsData
    /// </summary>
    /// <param name="datas"></param>
    public void LoadScenesData(List<ScenePlantData> datas)
    {
        _allTreesData.SetScenesDataOnLoad(datas);
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

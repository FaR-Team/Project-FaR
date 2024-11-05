using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class DirtSaver : Saver<DirtData, SaveDirtData>
{
    public static DirtSaver instance;

    private AllDirtsData allDirtsData = new AllDirtsData();

    private List<SaveDirtData> dirts = new List<SaveDirtData>();

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

            allDirtsData.SaveQueue(SceneManager.GetActiveScene().name);
            SaverManager.Save(allDirtsData, isTemporarySave);
            allDirtsData.ClearAfterSave();

            this.LogSuccess("Successfully Saved dirts information in scene " + SceneManager.GetActiveScene().name );
        }
        catch (Exception e)
        {
            this.LogError("Failed Save Dirt. Reason: " + e);
        }
    }

    public override Task WriteSave(DirtData info)
    {
        allDirtsData.data.Enqueue(info);
        allDirtsData.counter++;
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Loads all previously saved Scene Datas in current AllDirtsData
    /// </summary>
    /// <param name="datas"></param>
    public void LoadScenesData(List<SceneDirtData> datas)
    {
        allDirtsData.SetScenesDataOnLoad(datas);
    }

    private async Task SaveDirts()
    {
        foreach (var dirt in dirts)
        {
            await dirt.SaveData();
        }
    }

    public override void AddSavedObject(SaveDirtData dirtData)
    {
        Debug.Log("Added dirt to save in DirtSaver");
        dirts.Add(dirtData);
    }

    public override void RemoveSavedObject(SaveDirtData dirtData)
    {
        dirts.Remove(dirtData);
    }
}

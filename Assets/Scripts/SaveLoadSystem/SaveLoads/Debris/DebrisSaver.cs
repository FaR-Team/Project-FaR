using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class DebrisSaver : Saver<DebrisData, SaveDebrisData>
{
    public static DebrisSaver instance;

    private AllDebrisData allDebrisData = new AllDebrisData();
    private List<SaveDebrisData> debrisList = new List<SaveDebrisData>();

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
    }

    protected async override void SaveAllData(bool isTemporarySave)
    {
        try
        {
            await SaveDebrisList();

            allDebrisData.SaveQueue(SceneManager.GetActiveScene().name);
            SaverManager.Save(allDebrisData, isTemporarySave);
            allDebrisData.ClearAfterSave();
        }
        catch (Exception e)
        {
            this.LogError("Failed Save Debris. Reason: " + e);
        }
    }

    public override Task WriteSave(DebrisData info)
    {
        allDebrisData.data.Enqueue(info);
        allDebrisData.counter++;
        return Task.CompletedTask;
    }
    
    public void LoadScenesData(List<SceneDebrisData> datas)
    {
        allDebrisData.SetScenesDataOnLoad(datas);
    }

    private async Task SaveDebrisList()
    {
        foreach (var debris in debrisList)
        {
            await debris.SaveData();
        }
    }

    public override void AddSavedObject(SaveDebrisData debrisData)
    {
        debrisList.Add(debrisData);
    }

    public override void RemoveSavedObject(SaveDebrisData debrisData)
    {
        debrisList.Remove(debrisData);
    }
}

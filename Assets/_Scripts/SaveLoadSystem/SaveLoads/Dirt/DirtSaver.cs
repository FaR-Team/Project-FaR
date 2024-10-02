using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

public class DirtSaver : Saver<DirtData, SaveDirtData>
{
    public static DirtSaver instance;

    private AllDirtsData allDirtsData = new AllDirtsData();

    private List<SaveDirtData> dirts = new List<SaveDirtData>();

    private void Awake()
    {
        instance = this;
    }
    protected async override void SaveAllData(bool isTemporarySave)
    {
        try
        {
            await SaveDirts();

            allDirtsData.SaveQueue();
            SaverManager.Save(allDirtsData, isTemporarySave);

            this.Log("Successfully Saved dirts information");
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

    private async Task SaveDirts()
    {
        foreach (var dirt in dirts)
        {
            await dirt.SaveData();
        }
    }

    public override void AddSavedObject(SaveDirtData dirtData)
    {
        dirts.Add(dirtData);
    }

    public override void RemoveSavedObject(SaveDirtData dirtData)
    {
        dirts.Remove(dirtData);
    }
}

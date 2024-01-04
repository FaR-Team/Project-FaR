using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ChestSaver : Saver<ChestData, ChestDataSaver>
{
    public static ChestSaver Instance;

    private AllChestSystems allChests = new AllChestSystems();
    private List<ChestDataSaver> chestDataSavers = new List<ChestDataSaver>();
    private void Awake()
    {
        Instance = this;
    }

    protected override async void SaveAllData(bool isTemporarySave)
    {
        try
        {
            await SaveInvs();

            allChests.SaveQueue();
            SaverManager.Save(allChests, isTemporarySave);
            
            Debug.Log("Successfully Saved Chests");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed Save Chests. Reason: {e}");
        }
    }

    private async Task SaveInvs()
    {
        foreach (var chestDataSaver in chestDataSavers)
        {
            await chestDataSaver.SaveData();
        }
    }

    public override Task WriteSave(ChestData invSystem)
    {
        allChests.data.Enqueue(invSystem);
        allChests.dataCounter++;

        return Task.CompletedTask;
    }

    public override void AddSavedObject(ChestDataSaver chestDS)
    {
        chestDataSavers.Add(chestDS);
    }

    public override void RemoveSavedObject(ChestDataSaver chestDS)
    {
        chestDataSavers.Remove(chestDS);
    }
}

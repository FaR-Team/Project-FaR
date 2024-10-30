using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class ChestSaver : Saver<ChestData, ChestDataSaver>
{
    public static ChestSaver instance;

    private AllChestSystems allChests = new();
    private List<ChestDataSaver> chestDataSavers = new();

    public static DummyLogger logger;

    private void Awake()
    {
        if(instance != null && instance != this) Destroy(this);
        else instance = this;
        
        logger = GameObject.FindObjectOfType<DummyLogger>();
        if (logger == null)
        {
            GameObject loggerObject = new GameObject("DataSaverLogger");
            logger = loggerObject.AddComponent<DummyLogger>();
            UnityEngine.Object.DontDestroyOnLoad(loggerObject);
        }
    }

    protected override async void SaveAllData(bool isTemporarySave)
    {
        try
        {
            await SaveInvs();

            allChests.SaveQueue(SceneManager.GetActiveScene().name);
            SaverManager.Save(allChests, isTemporarySave);
            allChests.ClearAfterSave();

            this.LogSuccess("Successfully Saved Chests in scene " + SceneManager.GetActiveScene().name);
        }
        catch (Exception e)
        {
            this.LogError($"Failed Save Chests. Reason: {e}");
        }
    }

    private async Task SaveInvs()
    {
        foreach (var chestDataSaver in chestDataSavers)
        {
            await chestDataSaver.SaveData();
        }
    }
    
    /// <summary>
    /// Loads all previously saved Scene Datas in current AllChestsData
    /// </summary>
    /// <param name="datas"></param>
    public void LoadScenesData(List<SceneChestData> datas)
    {
        allChests.SetScenesDataOnLoad(datas);
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

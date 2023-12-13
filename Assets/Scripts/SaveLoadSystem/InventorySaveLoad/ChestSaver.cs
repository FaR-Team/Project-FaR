using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ChestSaver : MonoBehaviour
{
    public static ChestSaver Instance;

    private AllChestSystems allInventorySystems;
    private List<ChestDataSaver> chestDataSavers = new List<ChestDataSaver>();
    private void Awake()
    {
        Instance = this;
        allInventorySystems = new AllChestSystems();
    }

    protected void Start()
    {
        Cama.Instance.SaveDataEvent.AddListener(SaveAllData);
    }

    protected async void SaveAllData(bool isTemporarySave)
    {
        try
        {
            await SaveInvs();
            SaverManager.Save(allInventorySystems, isTemporarySave);
            Debug.Log("Successfully Saved Chests");
          //  Debug.Log(allInventorySystems.data.Keys.Count);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed Save Inventories. Reason: " + e);
        }
    }

    private async Task SaveInvs()
    {
        foreach (var chestDataSaver in chestDataSavers)
        {
            await chestDataSaver.SaveData();
        }
    }

    public Task WriteSave(ChestData invSystem)
    {
        print(invSystem);
        allInventorySystems.data.Add(invSystem);
        allInventorySystems.dataCounter++;
        return Task.CompletedTask;
    }

    public void AddSavedObject(ChestDataSaver chestDS)
    {
        chestDataSavers.Add(chestDS);
    }

    public void RemoveSavedObject(ChestDataSaver chestDS)
    {
        print("removed save");
        chestDataSavers.Remove(chestDS);
    }
}

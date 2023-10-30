using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DirtSaver : MonoBehaviour
{
    public static DirtSaver instance;

    private AllDirtsData allDirtsData;

    private List<SaveDirtData> dirts;

    private void Awake()
    {
        allDirtsData = new AllDirtsData(0);
        instance = this;
        dirts = new List<SaveDirtData>();
    }
    private void Start()
    {
        Cama.Instance.SaveDataEvent.AddListener(SaveAllData);
    }

    public Task WriteSave(DirtSaveData info)
    {
        allDirtsData.data.Enqueue(info);
        allDirtsData.DirtCounter++;
        return Task.CompletedTask;
    }

    public async void SaveAllData(bool isTemporarySave)
    {
        Debug.Log("SAVING");

        try
        {
            await SaveDirts();

            allDirtsData.SaveQueue();
            SaverManager.Save(allDirtsData, isTemporarySave);
            Debug.Log("Successfully Saved dirts data");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed Save Dirt. Reason: " + e);
        }
    }
    private async Task SaveDirts()
    {
        foreach (var dirt in dirts)
        {
            await dirt.SaveData();
        }
    }
    public void RemoveDirt(SaveDirtData dirt)
    {
        dirts.Remove(dirt);
    }

    public void AddDirt(SaveDirtData dirt)
    {
        Debug.Log("adding Dirt");

        dirts.Add(dirt);
    }
}

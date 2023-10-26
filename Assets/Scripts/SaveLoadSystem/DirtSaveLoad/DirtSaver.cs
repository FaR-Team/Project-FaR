using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DirtSaver : MonoBehaviour
{
    public static DirtSaver instance;

    private AllDirtsData allDirtsData;

    List<Task> tasks = new List<Task>();

    private void Awake()
    {
        allDirtsData = new AllDirtsData(0);
        instance = this;
    }
    private void Start()
    {
        Cama.Instance.SaveDataEvent.AddListener(SaveAllData);
    }

    public Task  WriteSave(DirtSaveData info)
    {
        allDirtsData.data.Enqueue(info);
        allDirtsData.DirtCounter++;
        return Task.CompletedTask;
    }

    public async void SaveAllData(bool isTemporarySave)
    {
        Debug.Log("SAVING");

        await Task.WhenAll(tasks);

        allDirtsData.SaveQueue();
        SaverManager.Save(allDirtsData, isTemporarySave);

        Debug.Log("Succesfully Saved");
    }

    public void AddTask(Task task)
    {
        tasks.Add(task);

        Debug.Log("added task");
    }

    public void RemoveTask(Task task)
    {
        tasks.Remove(task);
    }
}

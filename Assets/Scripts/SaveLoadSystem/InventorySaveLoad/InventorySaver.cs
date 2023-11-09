using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class InventorySaver : MonoBehaviour 
{
    public static InventorySaver Instance;

    private AllInventorySystems AllInventorySystems = new AllInventorySystems(0);
    private List<ContainerDataSaver> containerDataSavers = new List<ContainerDataSaver>();
    private void Awake()
    {
        Instance = this;
    }
    protected void Start()
    {
        Cama.Instance.SaveDataEvent.AddListener(SaveAllData);
    }
    protected async void SaveAllData(bool isTemporarySave)
    {
        Debug.Log("SAVING");

        try
        {
            await SaveInvs();

            AllInventorySystems.SaveDict();

            SaverManager.Save(AllInventorySystems, isTemporarySave);
            Debug.Log("Successfully Saved Inventories");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed Save Inventories. Reason: " + e);
        }
    }
    private async Task SaveInvs()
    {
        foreach (var containerDS in containerDataSavers)
        {
            await containerDS.SaveData();
        }
    }
    public Task WriteSave(InventorySystem invSystem, string id)
    {
        AllInventorySystems.data.Add(id, invSystem);
        AllInventorySystems.dataCounter++;
        return Task.CompletedTask;
    }
    public void AddSavedObject(ContainerDataSaver conteinerDSaver)
    {
        containerDataSavers.Add(conteinerDSaver);
    }

    public void RemoveSavedObject(ContainerDataSaver conteinerDSaver)
    {
        containerDataSavers.Remove(conteinerDSaver);
    }
}

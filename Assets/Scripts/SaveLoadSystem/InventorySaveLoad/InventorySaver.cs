using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class InventorySaver : MonoBehaviour 
{
    public static InventorySaver Instance;

    private AllInventorySystems allInventorySystems;
    private List<ContainerDataSaver> containerDataSavers = new List<ContainerDataSaver>();
    private void Awake()
    {
        Instance = this;
        allInventorySystems = new AllInventorySystems();
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
            Debug.Log("Successfully Saved Inventories");
            Debug.Log(allInventorySystems.data.Keys.Count);
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
        print((invSystem, id));
        allInventorySystems.data.TryAdd(id, invSystem);
        allInventorySystems.dataCounter++;
        return Task.CompletedTask;
    }

    public void AddSavedObject(ContainerDataSaver containerDSaver)
    { 
        containerDataSavers.Add(containerDSaver);
    }

    public void RemoveSavedObject(ContainerDataSaver conteinerDSaver)
    {

       print("removed save");
        containerDataSavers.Remove(conteinerDSaver);
    }
}

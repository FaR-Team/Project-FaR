using System.Threading.Tasks;
using UnityEngine;

public class InventoryDataSaver : ContainerDataSaver
{
    protected override void Awake()
    {
        uniqueiD = GetComponent<UniqueID>();
        chest = GetComponent<Container>();
    }
    
    protected override void Start()
    {
        InventorySaver.Instance.AddSavedObject(this);
    }

    public override async Task SaveData()
    {
        await InventorySaver.Instance.WriteSave(chest.PrimaryInventorySystem, uniqueiD.ID);
    }
}
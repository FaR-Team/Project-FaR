using UnityEngine;
using System.Threading.Tasks;

public class ChestDataSaver : ContainerDataSaver
{
    protected override void Awake()
    {
        uniqueiD = GetComponent<UniqueID>();
        container = GetComponent<Container>();
    }
    
    protected override void Start()
    {
        InventorySaver.Instance.AddSavedObject(this);
    }

    public override async Task SaveData()
    {
        await InventorySaver.Instance.WriteSave(container.PrimaryInventorySystem, uniqueiD.ID);
    }

}
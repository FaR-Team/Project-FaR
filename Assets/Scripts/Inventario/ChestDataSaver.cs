using UnityEngine;
using System.Threading.Tasks;

public class ChestDataSaver : DataSaver<Cofre>
{
    protected override void Start()
    {
        ChestSaver.Instance.AddSavedObject(this);
    }

    public override async Task SaveData()
    {
        ChestData chestData = new ChestData(gameObject.transform, objectToSave.PrimaryInventorySystem);

        await ChestSaver.Instance.WriteSave(chestData);
    }

    protected override void SetObserver()
    {
        throw new System.NotImplementedException();
    }
}

public interface IDataSaver
{
    public Task SaveData();
    
}

[SerializeField]
public class ChestData
{
    public Transform position;
    public InventorySystem inventorySystem;

    public ChestData(Transform position, InventorySystem inventorySystem)
    {
        this.position = position;
        this.inventorySystem = inventorySystem;
    }
}
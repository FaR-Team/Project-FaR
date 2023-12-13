using System.Threading.Tasks;
using UnityEngine;

public abstract class ContainerDataSaver : DataSaver<Container>
{
    protected Container chest;

    protected override void SetObserver()
    {
        throw new System.NotImplementedException();
    }

    public override async Task SaveData()
    {
        print("4");
        await InventorySaver.Instance.WriteSave(chest.PrimaryInventorySystem, uniqueiD.ID);
    }
}

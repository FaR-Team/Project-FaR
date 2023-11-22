using System.Threading.Tasks;
using UnityEngine;

public abstract class ContainerDataSaver : MonoBehaviour
{
    protected Container container;
    protected UniqueID uniqueiD;
    protected virtual void Awake()
    {
        uniqueiD = GetComponent<UniqueID>();
        container = GetComponent<Container>();
    }
    protected virtual void Start()
    {
        InventorySaver.Instance.AddSavedObject(this);
    }

    public virtual async Task SaveData()
    {
        print("4");
        await InventorySaver.Instance.WriteSave(container.PrimaryInventorySystem, uniqueiD.ID);
    }
}
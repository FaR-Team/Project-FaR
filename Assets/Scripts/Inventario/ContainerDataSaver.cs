using System.Threading.Tasks;
using UnityEngine;

public class ContainerDataSaver : MonoBehaviour
{
    Container container;
    UniqueID uniqueiD;
    private void Awake()
    {
        uniqueiD = GetComponent<UniqueID>();
        container = GetComponent<Container>();
    }
    private void Start()
    {
        InventorySaver.Instance.AddSavedObject(this);
    }

    public async Task SaveData()
    {
        await InventorySaver.Instance.WriteSave(container.PrimaryInventorySystem, uniqueiD.ID);
    }
}

using System;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public abstract class Container : MonoBehaviour //ex inventoryholder
{
    //objeto estilo conteiner.
    [SerializeField] private int tamañoInventario;
    [SerializeField] protected InventorySystem inventorySystem;
    [SerializeField] protected int offset = 10;
    [SerializeField] protected int _gold;

    public int Offset => offset;

    public InventorySystem PrimaryInventorySystem => inventorySystem;

    protected virtual void Awake() // Carga el inventario cn lo que se habia guardado anteriormente
    {
        LoadInventory(false);
    }

    protected void LoadInventory(bool isTemporary)
    {
        string id = GetComponent<UniqueID>().ID;
        try
        {
            AllInventorySystems allData = LoadAllInvsData.GetData(isTemporary);
            InventorySystem preresult = allData.data[id];
            inventorySystem = new InventorySystem(preresult.inventorySlots, preresult.Gold, preresult.hotbarAbilitySlots);
            inventorySystem = allData.data[id];
            print("loaded");
        }
        catch(Exception e)
        {
            print("NOT loaded:" + e);
            inventorySystem = new InventorySystem(tamañoInventario, _gold);
            //Debug.Log(inventorySystem.tamañoInventario + " conteiner");
        }
    }

}

[System.Serializable]
public struct InventorySaveData
{
    public InventorySystem InvSystem;
    public Vector3 Position;
    public Quaternion Rotation;

    public InventorySaveData(InventorySystem _invSystem, Vector3 _position, Quaternion _rotation)
    {
        InvSystem = _invSystem;
        Position = _position;
        Rotation = _rotation;
    }

    public InventorySaveData(InventorySystem _invSystem)
    {
        InvSystem = _invSystem;
        Position = Vector3.zero;
        Rotation = Quaternion.identity;
    }
}
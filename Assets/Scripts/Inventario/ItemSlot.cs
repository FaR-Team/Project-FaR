using FaRUtils.Systems.ItemSystem;
using UnityEngine;

[System.Serializable]
public abstract class ItemSlot : ISerializationCallbackReceiver
{
    [SerializeField] protected InventoryItemData itemData; //referencia a los datos del item
    [SerializeField] protected int _itemID = -1;
    [SerializeField] protected int stackSize; //La cantidad de items que hay en el slot

    public InventoryItemData ItemData
    {
        get
        {
            if (itemData == null && _itemID != -1)
            {
                var db = Resources.Load<Database>("Database");
                if (db != null) itemData = db.GetItem(_itemID);
            }
            return itemData;
        }
    }
    public int StackSize => stackSize;
    public int CantidadMáxima = 100;

    public void ClearSlot() //Limpiar un Slot
    {
        if (stackSize > 0) return;

        ForcedClearSlot();
    }

    public void ForcedClearSlot() //Fuerza la Limpieza de un Slot
    {
        itemData = null;
        _itemID = -1;
        stackSize = -1;
    }

    public void AssignItem(InventorySlot invSlot) //Asignar un item directamente a un slot
    {
        if (itemData == invSlot.ItemData) //El slot contiene el mismo item?
        {
            AddToStack(invSlot.StackSize); 
        }
        else //Sobreescribir slot con el que estamos pasándole
        {
            itemData = invSlot.itemData;
            _itemID = itemData != null ? itemData.ID : -1;
            stackSize = 0;
            AddToStack(invSlot.StackSize);
        }
    }

    public void AssignItem(InventoryItemData data, int amount)
    {
        if (itemData == data) AddToStack(amount);
        else 
        {
            itemData = data;
            _itemID = data != null ? data.ID : -1;
            stackSize = 0;
            AddToStack(amount);
        }
    }

    public void AddToStack(int amount)
    {
        stackSize += amount;
    }

    public void SacarDeStack(int amount)
    {
        stackSize -= amount;
    }

    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
    }
}

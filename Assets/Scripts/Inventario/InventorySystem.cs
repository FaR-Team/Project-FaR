using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class InventorySystem
{
    [SerializeField] private List<InventorySlot> inventorySlots;
    [SerializeField] private int _gold;

    public int Gold => _gold;

    public List<InventorySlot> InventorySlots => inventorySlots;
    public int tamañoInventario => inventorySlots.Count;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public InventorySystem(int tamaño) //Constructo que coloca la cantidad de Slots
    {
        _gold = 0;
        CreateInventory(tamaño);
    }

    public InventorySystem(int tamaño, int gold)
    {
        _gold = gold;
        CreateInventory(tamaño);
    }

    private void CreateInventory(int tamaño)
    {
        inventorySlots = new List<InventorySlot>(tamaño);

        for (int i = 0; i < tamaño; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    public bool AñadirAInventario(InventoryItemData itemAAñadir, int cantidadParaAñadir)
    {
        if (ContieneItem(itemAAñadir, out List<InventorySlot> invSlot)) //Revisa si el item ya existe en el inventario
        {
            foreach (var slot in invSlot)
            {
                if (slot.EnoughRoomLeftInStack(cantidadParaAñadir))
                {
                    slot.AddToStack(cantidadParaAñadir);
                    OnInventorySlotChanged?.Invoke(slot);
                    return true;
                }
            }
        }

        if (HaySlotLibre(out InventorySlot SlotLibre)) //Busca el primer slot libre
        {
            if (SlotLibre.EnoughRoomLeftInStack(cantidadParaAñadir))
            {
                SlotLibre.UpdateInventorySlot(itemAAñadir, cantidadParaAñadir);
                OnInventorySlotChanged?.Invoke(SlotLibre);
                return true;
            }
            //Implementar sólo añadir lo que se peuda añadir al stack, y dejar el resto en otro stack libre
        }

        return false;
    }

    public bool ContieneItem(InventoryItemData itemAAñadir, out List<InventorySlot> invSlot) //alguno de los slots ya tiene este item?
    {
        invSlot = InventorySlots.Where(i => i.ItemData == itemAAñadir).ToList(); // Selecciona los slots que contienen el item, y los pone en una lista
        return !(invSlot == null);
    }

    public bool HaySlotLibre(out InventorySlot SlotLibre)
    {
        SlotLibre = InventorySlots.FirstOrDefault(i => i.ItemData == null); //Busca el primer slot libre
        return !(SlotLibre == null);
    }

    public bool CheckInventoryRemaining(Dictionary<InventoryItemData, int> shoppingCart)
    {
        var clonedSystem = new InventorySystem(this.tamañoInventario);

        for (int i = 0; i < tamañoInventario; i++)
        {
            clonedSystem.InventorySlots[i].AssignItem(this.InventorySlots[i].ItemData, this.InventorySlots[i].StackSize);
        }

        foreach (var kvp in shoppingCart)
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                if (!clonedSystem.AñadirAInventario(kvp.Key, 1)) return false;
            }
        }

        return true;
    }

    public void SpendGold(int basketTotal)
    {
        _gold -= basketTotal;
    }

    public void GainGold(int price)
    {
        _gold += price;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class InventorySystem
{
    public List<InventorySlot> inventorySlots;
    public int _gold;

    public List<InventorySlot> hotbarAbilitySlots;
    public int Gold => _gold;

    public List<InventorySlot> InventorySlots => inventorySlots;
    public int tamañoInventario => inventorySlots.Count;

    public UnityAction<InventorySlot> OnInventorySlotChanged;
    public event Action<int> OnGoldAmountChanged;

    public InventorySystem(int tamaño) //Constructo que coloca la cantidad de Slots
    {
        CreateInventory(tamaño);
    }
    public InventorySystem(InventorySystem inventoryData)
    {
        this.inventorySlots = inventoryData.inventorySlots;
        this._gold = inventoryData._gold;

    }
    public InventorySystem(int tamaño, int gold)
    {
        _gold = gold;
        CreateInventory(tamaño);
    }

    public InventorySystem(List<InventorySlot> inventorySlots, int gold, List<InventorySlot> hotbarAbilitySlots)
    {
        this.inventorySlots = inventorySlots;
        _gold = gold;
        this.hotbarAbilitySlots = hotbarAbilitySlots;
    }

    private void CreateInventory(int tamaño)
    {
        inventorySlots = new List<InventorySlot>(tamaño);

        for (int i = 0; i < tamaño; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    public void ClearInventory()
    {
        inventorySlots.ForEach(slot => slot.ForcedClearSlot());
    }

    public bool AddToHotbarAbility(InventoryItemData itemToAdd, int amount)
    {
        if (ContieneTool(itemToAdd)) return false;

        if (HaySlotLibreEnLaSpecialHotbar(out InventorySlot SlotLibre))
        {
            if (SlotLibre.EnoughRoomLeftInStack(amount))
            {
                SlotLibre.UpdateInventorySlot(itemToAdd, amount);
                OnInventorySlotChanged?.Invoke(SlotLibre);
                return true;
            }
        }
        return false;
    }

    public bool ContieneTool(InventoryItemData itemToAdd)
    {
        var hotbarSlots = hotbarAbilitySlots.Where(i => i.ItemData == itemToAdd).ToList();
        return !(hotbarSlots == null);
    }
    
    public bool AddToInventory(InventoryItemData itemAAñadir, int cantidadParaAñadir)
    {
        if (HasItem(itemAAñadir, out List<InventorySlot> invSlot))
        {
            foreach (var slot in invSlot)
            {
                if (slot.IsBlocked) continue;

                if (slot.EnoughRoomLeftInStack(cantidadParaAñadir))
                {
                    slot.AddToStack(cantidadParaAñadir);
                    OnInventorySlotChanged?.Invoke(slot);
                    return true;
                }
            }
        }

        if (HaySlotLibre(out InventorySlot SlotLibre))
        {
            if (SlotLibre.IsBlocked) return false;

            if (SlotLibre.EnoughRoomLeftInStack(cantidadParaAñadir))
            {
                SlotLibre.UpdateInventorySlot(itemAAñadir, cantidadParaAñadir);
                OnInventorySlotChanged?.Invoke(SlotLibre);
                return true;
            }
        }

        return false;
    }

    public bool HasItem(InventoryItemData itemAAñadir, out List<InventorySlot> invSlot) //alguno de los slots ya tiene este item?
    {
        invSlot = InventorySlots.Where(i => i.ItemData == itemAAñadir).ToList(); // Selecciona los slots que contienen el item, y los pone en una lista
        return !(invSlot == null);
    }

    public bool HaySlotLibreEnLaSpecialHotbar(out InventorySlot SlotLibre)
    {
        SlotLibre = hotbarAbilitySlots.FirstOrDefault(i => i.ItemData == null); //Busca el primer slot libre
        return !(SlotLibre == null);
    }

    public bool HaySlotLibre(out InventorySlot SlotLibre)
    {
        SlotLibre = InventorySlots.FirstOrDefault(i => i.ItemData == null && !i.IsBlocked); //Busca el primer slot libre y no bloqueado
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
                if (!clonedSystem.AddToInventory(kvp.Key, 1)) return false;
            }
        }

        return true;
    }

    public void SpendGold(int basketTotal)
    {
        _gold -= basketTotal;
        OnGoldAmountChanged?.Invoke(_gold);
    }

    public void GainGold(int price)
    {
        _gold += price;
        OnGoldAmountChanged?.Invoke(_gold);
    }
}

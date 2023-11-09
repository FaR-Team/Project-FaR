using System.Collections.Generic;
using UnityEngine;

public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField] protected InventorySlot_UIBasic slotPrefab;

    public void RefreshDynamicInventory(InventorySystem invToDisplay, int offset)
    {
        inventorySystem = invToDisplay;
        if (slotDictionary.Count > 0)
        {
            
            UpdateSlots(inventorySystem, offset);
        }
        else
        {
            CreateSlots(inventorySystem, offset);
           // print(inventorySlots.Count);
        }

        inventorySystem.OnInventorySlotChanged += UpdateSlot;
    }
    public void UpdateSlots(InventorySystem invToDisplay, int offset)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlot_UIBasic slot = inventorySlots[i];
            slot.UpdateUISlot(invToDisplay.InventorySlots[i + offset]);
            //print(i);
        }
    }
    public override void CreateSlots(InventorySystem invToDisplay, int offset)
    {
        slotDictionary = new Dictionary<InventorySlot_UIBasic, InventorySlot>();

        if (invToDisplay == null) return;

        for (int i = offset; i < invToDisplay.tamañoInventario; i++)
        {
            InventorySlot_UIBasic uiSlot = Instantiate(slotPrefab, transform);
            slotDictionary.Add(uiSlot, invToDisplay.InventorySlots[i]);
            uiSlot.Init(invToDisplay.InventorySlots[i]);
            uiSlot.UpdateUISlot();
            inventorySlots.Add(uiSlot);
        }
    }

    private void OnDisable()
    {
        if (inventorySystem != null)
        {
            inventorySystem.OnInventorySlotChanged -= UpdateSlot;
        }
    }
}

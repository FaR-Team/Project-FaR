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
    public virtual void UpdateSlots(InventorySystem invToDisplay, int offset)
    {
        // If requested inventory has more slots than the UI inventory..-
        if (invToDisplay.InventorySlots.Count > this.inventorySlots.Count) 
        {
            Debug.Log("More slots in inv");
           
            for (int i = 0; i < invToDisplay.InventorySlots.Count; i++)
            {
                if (i < inventorySlots.Count)
                {
                    slotDictionary[inventorySlots[i]] = invToDisplay.InventorySlots[i + offset];
                    inventorySlots[i].Init(invToDisplay.InventorySlots[i + offset]);
                    //inventorySlots[i].UpdateUISlot(invToDisplay.InventorySlots[i + offset]);
                }
                else CreateAndFillSlot(invToDisplay.inventorySlots[i+offset]); // Create new slots if necessary
            }
        }
        else // If there's already more UI slots than required for current inventory...
        {
            Debug.Log("More slots in UI");
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (i >= invToDisplay.InventorySlots.Count)
                {
                    inventorySlots[i].gameObject.SetActive(false); // Deactivate unnecessary slots
                    continue;
                }
                slotDictionary[inventorySlots[i]] = invToDisplay.InventorySlots[i + offset];
                inventorySlots[i].Init(invToDisplay.InventorySlots[i + offset]);
                //inventorySlots[i].UpdateUISlot(invToDisplay.InventorySlots[i + offset]);
                inventorySlots[i].gameObject.SetActive(true);
            }
        }
    }
    public override void CreateSlots(InventorySystem invToDisplay, int offset)
    {
        slotDictionary = new Dictionary<InventorySlot_UIBasic, InventorySlot>();
        if (invToDisplay == null) return;

        for (int i = 0; i < invToDisplay.tamañoInventario; i++)
        {
            CreateAndFillSlot(invToDisplay.InventorySlots[i]);
        }
        
    }

    public void CreateAndFillSlot(InventorySlot invSlot)
    {
        InventorySlot_UIBasic uiSlot = Instantiate(slotPrefab, transform);
        slotDictionary.Add(uiSlot, invSlot);
        uiSlot.Init(invSlot);
        uiSlot.UpdateUISlot();
        inventorySlots.Add(uiSlot);
    }

    private void OnDisable()
    {
        if (inventorySystem != null)
        {
            inventorySystem.OnInventorySlotChanged -= UpdateSlot;
        }
    }
}

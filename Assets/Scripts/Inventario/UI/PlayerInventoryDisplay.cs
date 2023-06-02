using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryDisplay : InventoryDisplay
{
    [SerializeField] private PlayerInventoryHolder playerInventoryHolder;
    [SerializeField] public InventorySlot_UI[] slots;

    protected virtual void OnEnable() {
        PlayerInventoryHolder.OnPlayerInventoryChanged += RefreshStaticDisplay;
    }

    protected virtual void OnDisable() {
        PlayerInventoryHolder.OnPlayerInventoryChanged -= RefreshStaticDisplay;
        inventorySystem.OnInventorySlotChanged -= UpdateSlot;
    }

    public void RefreshStaticDisplay() {
        if (playerInventoryHolder != null)
        {
            inventorySystem = playerInventoryHolder.PrimaryInventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }
        else
        {
            Debug.LogError($"No hay inventario asignado a {this.gameObject}");
        }

        if(slotDictionary == null) AssignSlot(inventorySystem, 0);

        for (int i = 0; i < inventorySystem.tamañoInventario; i++)
        {
            slots[i].UpdateUISlot();
        }
    }

    protected override void Start()
    {
        base.Start();
        //AssignSlot(inventorySystem, 0);
        RefreshStaticDisplay();
    }

    public override void AssignSlot(InventorySystem invToDisplay, int offset)
    {
        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();

        for (int i = 0; i < playerInventoryHolder.Offset; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
            slots[i].UpdateUISlot();
        }
    }

    public void UpgradeInventory(int level)
    {
        
    }
}

using System.Collections.Generic;
using UnityEngine;

public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField] private PlayerInventoryHolder playerInventoryHolder;
    [SerializeField] public InventorySlot_UIBasic[] slots;

    protected virtual void OnEnable() {
        PlayerInventoryHolder.OnPlayerInventoryChanged += RefreshStaticDisplay;
    }

    protected virtual void OnDisable() {
        PlayerInventoryHolder.OnPlayerInventoryChanged -= RefreshStaticDisplay;
    }

    private void RefreshStaticDisplay() {
        if (playerInventoryHolder != null)
        {
            inventorySystem = playerInventoryHolder.PrimaryInventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }
        else
        {
            Debug.LogError($"No hay inventario asignado a {this.gameObject}");
        }

        CreateSlots(inventorySystem, 0);
    }

    protected virtual void Start()
    {
        RefreshStaticDisplay();
    }

    public override void CreateSlots(InventorySystem invToDisplay, int offset)
    {
        slotDictionary = new Dictionary<InventorySlot_UIBasic, InventorySlot>();

     //  Debug.Log(inventorySystem.tama�oInventario + "crear static");
        for (int i = 0; i < playerInventoryHolder.Offset; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
        }
    }
}
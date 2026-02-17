using System.Collections.Generic;
using UnityEngine;
using Utils;

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
        if (playerInventoryHolder == null) playerInventoryHolder = PlayerInventoryHolder.instance;

        if (playerInventoryHolder != null)
        {
            if (inventorySystem != null) inventorySystem.OnInventorySlotChanged -= UpdateSlot;
            inventorySystem = playerInventoryHolder.PrimaryInventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }
        else
        {
            this.LogWarning($"No hay inventario asignado a {this.gameObject}. No se puede refrescar.");
            return;
        }

        CreateSlots(inventorySystem, 0);
    }

    protected virtual void Start()
    {
        RefreshStaticDisplay();
    }

    public override void CreateSlots(InventorySystem invToDisplay, int offset)
    {
        if (playerInventoryHolder == null) playerInventoryHolder = PlayerInventoryHolder.instance;
        if (playerInventoryHolder == null || inventorySystem == null) return;

        slotDictionary = new Dictionary<InventorySlot_UIBasic, InventorySlot>();
        inventorySlots = new List<InventorySlot_UIBasic>();

        int slotCount = Mathf.Min(slots.Length, playerInventoryHolder.Offset);

        for (int i = 0; i < slotCount; i++)
        {
            if (slots[i] == null) continue;

            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
            slots[i].SetParentDisplay(this);
            inventorySlots.Add(slots[i]);
        }
    }
}
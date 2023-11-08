using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx.Triggers;

public class DynamicInventoryDisplayBackpack : DynamicInventoryDisplay
{
    public List<InventorySlot_UIBasic> inventorySlots;

    public static DynamicInventoryDisplayBackpack instance;

    private void Awake()
    {
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void AssignSlot(InventorySystem invToDisplay, int offset)
    {
        slotDictionary = new Dictionary<InventorySlot_UIBasic, InventorySlot>();

        inventorySlots = new List<InventorySlot_UIBasic>();

        if (invToDisplay == null) return;

        for (int i = offset; i < invToDisplay.tamañoInventario; i++)
        {
            InventorySlot_UI_Backpack uiSlot = (InventorySlot_UI_Backpack) Instantiate(slotPrefab, transform);
            slotDictionary.Add(uiSlot, invToDisplay.InventorySlots[i]);
            inventorySlots.Add(uiSlot);
            uiSlot.Init(invToDisplay.InventorySlots[i], EnumAPasar(i));
            uiSlot.UpdateUISlot();
        }
    }

    private TypesOfInventory EnumAPasar(int i)
    {
        TypesOfInventory _enum;
        if (i is 13 or 14 or 18 or 19 or 23 or 24)
        {
            _enum = TypesOfInventory.CAMISA;
        }
        else if(i is 28 or 29 or 33 or 34 or 38 or 39 )
        {
            _enum = TypesOfInventory.PANTALON;
        }
        else
        {
            _enum = TypesOfInventory.INVENTARIO;
        }
        return _enum;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField] protected InventorySlot_UI slotPrefab;
    protected override void Start()
    {
        base.Start();
    }

    public void RefreshDynamicInventory(InventorySystem invToDisplay, int offset)
    {
        ClearSlots();
        inventorySystem = invToDisplay;
        if (inventorySystem != null) 
        {
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }
        AssignSlot(invToDisplay, offset);
    }

    public override void AssignSlot(InventorySystem invToDisplay, int offset)
    {

        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();

        if (invToDisplay == null)
        {
            return;
        }

        for (int i = offset; i < invToDisplay.tamañoInventario; i++)
        {
            var uiSlot = Instantiate(slotPrefab, transform);
            slotDictionary.Add(uiSlot, invToDisplay.InventorySlots[i]);

            uiSlot.Init(invToDisplay.InventorySlots[i], EnumAPasar(i));
            uiSlot.UpdateUISlot();
        }
    }

    private TypesOfInventory EnumAPasar(int i)
    {
        TypesOfInventory _enum;
        if (i is 13 or 14 or 18 or 19 or 23 or 24)
        {
            _enum = TypesOfInventory.PANTALON;
        }
        else
        {
            _enum = TypesOfInventory.INVENTARIO;
        }
        return _enum;
    }

    private void ClearSlots()
    {
        foreach (var item in transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }

        if (slotDictionary != null)
        {
            slotDictionary.Clear();
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

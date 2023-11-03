using System;
using System.Collections;
using System.Collections.Generic;
using FaRUtils.Systems.ItemSystem;
using UnityEngine;

[System.Serializable]

public class InventorySlot : ItemSlot 
{

    public InventorySlot(InventoryItemData data, int amount) //Constructor para hacer un slot lleno
    {
        itemData = data;
        _itemID = itemData.ID;
        stackSize = amount;
    }

    public InventorySlot() //Constructor para hacer un slot vacío
    {
        ForcedClearSlot();
    }

    public void UpdateInventorySlot(InventoryItemData data, int amount)
    {
        itemData = data;
        _itemID = itemData.ID;
        stackSize = amount;
    }

    public bool EnoughRoomLeftInStack(int amountParaAñadir, out int amountRestante) // Calcula el espacio restante en el stack para ver si podemos añadir
    {
        amountRestante = CantidadMáxima - stackSize;

        return EnoughRoomLeftInStack(amountParaAñadir);
    }

    public bool EnoughRoomLeftInStack(int amountParaAñadir)
    {
        return (itemData == null || itemData != null && stackSize + amountParaAñadir <= CantidadMáxima);
    }

    public bool SplitStack(out InventorySlot splitStack)
    {
        if (stackSize <= 1) // Hay suficiente para dividir si no, da falso.
        {
            splitStack = null;
            return false;
        }

        int halfStack = Mathf.RoundToInt(stackSize / 2); //Si sí, calcular mitad del stack
        SacarDeStack(halfStack);

        splitStack = new InventorySlot(itemData, halfStack); //Crea una copia del slot con la mitad del stack
        return true;
    }


}

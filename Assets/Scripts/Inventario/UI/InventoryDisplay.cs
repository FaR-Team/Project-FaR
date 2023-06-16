using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] MouseItemData mouseInventoryItem;
    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlot_UIBase, InventorySlot> slotDictionary; //Diccionario de slots de UI y slots del inventario

    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlot_UIBase, InventorySlot> SlotDictionary => slotDictionary;
    public GameObject uiClickHandler;

    public abstract void AssignSlot(InventorySystem invToDisplay, int offset);

    protected virtual void Start()
    {

    }

    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        foreach (var slot in SlotDictionary)
        {
            if (slot.Value == updatedSlot)
            {
                slot.Key.UpdateUISlot(updatedSlot);
            }
        }
    }

    public void SlotClicked(InventorySlot_UIBase clickedUISlot)
    {
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        

        if (clickedUISlot.AssignedInventorySlot.ItemData != null && 
        mouseInventoryItem.AssignedInventorySlot.ItemData == null)
        {
         //Checkea si el jugador está apretando cShift, así dividimos el stack
            if (isShiftPressed && clickedUISlot.AssignedInventorySlot.SplitStack(out InventorySlot halfStackSlot))
            {
                mouseInventoryItem.UpdateMouseSlot(halfStackSlot);
                clickedUISlot.UpdateUISlot();
                //uiClickHandler.GetComponent<UiClickHandler>().isRightClick = false;
                return;
            }
            else
            {
            mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
            clickedUISlot.ClearSlot();
            return;
            }
        }

        // Si el slot clickeado no tiene un item y el mouse tiene un item, se agrega el item del mouse al slot
        if (clickedUISlot.AssignedInventorySlot.ItemData == null && 
        mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
            clickedUISlot.UpdateUISlot();

            mouseInventoryItem.ClearSlot();
            return;
        }

        // Revisar si ambos slots tienen un item, decidir que hacer:
        // 1. Si son el mismo item, los combinamos
            // La suma del slot y el mouse supera el stack máximo?; si es así, se combinan y se queda con el stack máximo
        // 2. Si son items diferentes, los intercambiamos
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && 
        mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseInventoryItem.AssignedInventorySlot.ItemData;
            
            //Ambos items son iguales, así que combinamos los stacks
            if (isSameItem && 
            clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize))
            {
                clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                clickedUISlot.UpdateUISlot();

                mouseInventoryItem.ClearSlot();
                return;
            }
            else if (isSameItem && 
            !clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int cantidadRestante))
            {
                if (cantidadRestante < 1) //El stack está completo, así que simplemente intercambiamos
                {
                    SwapSlots(clickedUISlot);
                }
                else
                {
                    int remainingOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - cantidadRestante;

                    clickedUISlot.AssignedInventorySlot.AddToStack(cantidadRestante);
                    clickedUISlot.UpdateUISlot();

                    var newItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, remainingOnMouse);
                    mouseInventoryItem.ClearSlot();
                    mouseInventoryItem.UpdateMouseSlot(newItem);
                    return;
                }
            }
            else if (!isSameItem)
            {
                SwapSlots(clickedUISlot);
                return;
            }
        }


    }

    private void SwapSlots(InventorySlot_UIBase clickedUISlot)
    {
        var clonedSlot = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, mouseInventoryItem.AssignedInventorySlot.StackSize);
        mouseInventoryItem.ClearSlot();

        mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);

        clickedUISlot.ClearSlot();
        clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
        clickedUISlot.UpdateUISlot();
    }
}

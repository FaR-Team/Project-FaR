using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryDisplay : MonoBehaviour
{
    /*
        Es una clase abstracta mono que deberia funcionar como "mostrar inventario".
    */
    [SerializeField] MouseItemData mouseInventoryItem;
    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlot_UIBasic, InventorySlot> slotDictionary = new Dictionary<InventorySlot_UIBasic, InventorySlot>(); //Diccionario de slots de UI y slots del inventario

    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlot_UIBasic, InventorySlot> SlotDictionary => slotDictionary;
    public GameObject uiClickHandler;

    public List<InventorySlot_UIBasic> inventorySlots = new List<InventorySlot_UIBasic>();

    public abstract void CreateSlots(InventorySystem invToDisplay, int offset);


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


    public void SlotClicked(InventorySlot_UIBasic clickedUISlot, bool isRightClick = false)
    {
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);

        if (isShiftPressed && clickedUISlot.AssignedInventorySlot.ItemData != null)
        {
            if (InventoryUIController.instance.currentContainer != null)
            {
                if (inventorySystem != InventoryUIController.instance.currentContainer)
                {
                    if (InventoryUIController.instance.currentContainer.AñadirAInventario(clickedUISlot.AssignedInventorySlot.ItemData, clickedUISlot.AssignedInventorySlot.StackSize))
                    {
                        clickedUISlot.ClearSlot();
                    }
                }
                else
                {
                    if (PlayerInventoryHolder.instance.AñadirAInventario(clickedUISlot.AssignedInventorySlot.ItemData, clickedUISlot.AssignedInventorySlot.StackSize))
                    {
                        clickedUISlot.ClearSlot();
                    }
                }
                return;
            }
            else
            {

            }
        }

        if (clickedUISlot.AssignedInventorySlot.ItemData != null &&
        mouseInventoryItem.AssignedInventorySlot.ItemData == null)
        {
            if (isRightClick && clickedUISlot.AssignedInventorySlot.SplitStack(out InventorySlot halfStackSlot))
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
            if (!isRightClick)
            {
                clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                clickedUISlot.UpdateUISlot();

                mouseInventoryItem.ClearSlot();
                return;
            }
            else
            {
                DropOneMouseSlot(clickedUISlot, true);
                return;
            }
        }

        // Revisar si ambos slots tienen un item, decidir que hacer:
        // 1. Si son el mismo item, los combinamos
        // La suma del slot y el mouse supera el stack máximo?; si es así, se combinan y se queda con el stack máximo
        // 2. Si son items diferentes, los intercambiamos
        if (clickedUISlot.AssignedInventorySlot.ItemData != null &&
        mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseInventoryItem.AssignedInventorySlot.ItemData;


            if (isSameItem)
            {
                if (!isRightClick)
                {
                    //Ambos items son iguales, así que combinamos los stacks
                    if (clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize))
                    {
                        clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                        clickedUISlot.UpdateUISlot();

                        mouseInventoryItem.ClearSlot();
                        return;
                    }
                    else if (!clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int cantidadRestante))
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
                }
                else
                {
                    //Dejamos uno del stack del mouse en el slot activo
                    if (clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(1))
                    {
                        DropOneMouseSlot(clickedUISlot);
                        return;
                    }
                }
            }
            else
            {
                SwapSlots(clickedUISlot);
                return;
            }
        }
    }

    private void DropOneMouseSlot(InventorySlot_UIBasic clickedUISlot, bool emptySlot = false)
    {
        int remainingOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - 1;
        if (!emptySlot) clickedUISlot.AssignedInventorySlot.AddToStack(1);
        else clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot.ItemData, 1);
        clickedUISlot.UpdateUISlot();

        if (remainingOnMouse > 0)
        {
            var newItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, remainingOnMouse);
            mouseInventoryItem.ClearSlot();
            mouseInventoryItem.UpdateMouseSlot(newItem);
        }
        else
        {
            mouseInventoryItem.ClearSlot();
        }
    }

    private void SwapSlots(InventorySlot_UIBasic clickedUISlot)
    {
        var clonedSlot = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, mouseInventoryItem.AssignedInventorySlot.StackSize);
        mouseInventoryItem.ClearSlot();

        mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);

        clickedUISlot.ClearSlot();
        clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
        clickedUISlot.UpdateUISlot();
    }

    public void SwapSlots(InventorySlot_UIBasic UISlot1, InventorySlot_UIBasic UISlot2)
    {
        InventorySlot clonedSlot = null;
        if (UISlot2.AssignedInventorySlot.ItemData != null) clonedSlot = new InventorySlot(UISlot2.AssignedInventorySlot.ItemData, UISlot2.AssignedInventorySlot.StackSize);

        UISlot2.ClearSlot();
        if (UISlot1.AssignedInventorySlot.ItemData != null)
        {
            UISlot2.AssignedInventorySlot.AssignItem(UISlot1.AssignedInventorySlot);
            UISlot2.UpdateUISlot();
        }

        UISlot1.ClearSlot();
        if (clonedSlot != null && clonedSlot.ItemData != null)
        {
            UISlot1.AssignedInventorySlot.AssignItem(clonedSlot);
            UISlot1.UpdateUISlot();
        }
    }

}

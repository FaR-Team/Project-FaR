using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MouseItemData : MonoBehaviour
{
    public Image ItemSprite;
    public TextMeshProUGUI ItemCount;
    public InventorySlot AssignedInventorySlot;
    public int _dropOffset;

    private void Awake()
    {
        ItemSprite.color = Color.clear;
        ItemCount.text = "";
    }

    public void UpdateMouseSlot(InventorySlot invSlot)
    {
        AssignedInventorySlot.AssignItem(invSlot);
        ItemSprite.sprite = invSlot.ItemData.Icono;
        ItemCount.text = invSlot.StackSize.ToString();
        ItemSprite.color = Color.white;
        ItemSprite.preserveAspect = true;
    }

    private void Update()
    {        
        if (AssignedInventorySlot.ItemData == null && 
            !PlayerInventoryHolder.isInventoryOpen) return;

        transform.position = Input.mousePosition;
    }

    public void ClearSlot()
    {
        AssignedInventorySlot.ForcedClearSlot();
        ItemSprite.color = Color.clear;
        ItemCount.text = "";
        ItemSprite.sprite = null;
    }

    public static bool IsPointerOverUIObject() //Revisa si el mouse está sobre un elemento de UI
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    } 
}

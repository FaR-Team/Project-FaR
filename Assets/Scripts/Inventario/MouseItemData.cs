using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class MouseItemData : MonoBehaviour
{
    public Image ItemSprite;
    public TextMeshProUGUI ItemCount;
    public InventorySlot AssignedInventorySlot;
    public int _dropOffset;

    private Transform _playerTransform;

    private void Awake()
    {
        ItemSprite.color = Color.clear;
        ItemCount.text = "";

        _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        
        if (_playerTransform = null) Debug.Log("No se encontró Jugador");
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
        //TODO: Añadir soporte para joystick
        
        /*if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
        if (AssignedInventorySlot.ItemData.ItemPrefab != null)
        {
        Instantiate(AssignedInventorySlot.ItemData.ItemPrefab, _playerTransform.position + _playerTransform.forward * _dropOffset, Quaternion.identity);
        }
        ClearSlot();
        }*/
        //Si tiene un item, que siga al mouse
        
        if (AssignedInventorySlot.ItemData == null) return;
        transform.position = Input.mousePosition;
    }

    public void ClearSlot()
    {
        AssignedInventorySlot.ClearSlot();
        ItemSprite.color = Color.clear;
        ItemCount.text = "";
        ItemSprite.sprite = null;
    }

    public static bool IsPointerOverUIObject() //Revisa si el mouse está sobre un elemto de UI
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    } 
}

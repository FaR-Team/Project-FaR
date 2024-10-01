using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] private Image _itemSprite;
    [SerializeField] private TextMeshProUGUI _itemName;
    //[SerializeField] private TextMeshProUGUI _itemCount;
    [SerializeField] private ShopSlot _assignedItemSlot;
    
    public ShopSlot AssignedItemSlot => _assignedItemSlot;

    [SerializeField] private Button _addItemToCartButton;
    [SerializeField] private Button _removeItemFromCartButton;

    private int _TempAmount;
    
    public ShopKeeperDisplay ParentDisplay { get; private set;}
    public float MarkUp { get; private set;}

    private void Awake() 
    {
        _itemSprite.sprite = null;
        _itemSprite.preserveAspect = true;
        _itemSprite.color = Color.clear;
        _itemName.text = "";
        //_itemCount.text = "";

        _addItemToCartButton?.onClick.AddListener(AddItemToCart);
        _removeItemFromCartButton?.onClick.AddListener(RemoveItemFromCart);
        ParentDisplay = transform.parent.GetComponentInParent<ShopKeeperDisplay>();
    }

    private void Update() 
    {
        UpdateUISlot();
    }

    public void Init(ShopSlot slot, float markUp)
    {
        _assignedItemSlot = slot;
        MarkUp = markUp;
        _TempAmount = slot.StackSize;
        UpdateUISlot();
    }

    private void UpdateUISlot()
    {
        if (_assignedItemSlot.ItemData != null)
        {
            _itemSprite.sprite = _assignedItemSlot.ItemData.Icono;
            _itemSprite.color = Color.white;
            //_itemCount.text = _assignedItemSlot.StackSize.ToString();
            var modifiedPrice = ShopKeeperDisplay.GetModifiedPrice(_assignedItemSlot.ItemData, ParentDisplay._BuyMulti, MarkUp);
            _itemName.text = $"{_assignedItemSlot.ItemData.Nombre} - {modifiedPrice}G";
        }
    }

    private void RemoveItemFromCart()
    {
        ParentDisplay.RemoveItemFromCart(this);
    }

    private void AddItemToCart()
    {
        //if (_tempAmount > 0)
        ParentDisplay.AddItemToCart(this);
    }
}

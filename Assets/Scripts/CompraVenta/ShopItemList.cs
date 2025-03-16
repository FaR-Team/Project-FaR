using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemList", menuName = "Jueguito Granjil/Compra-Venta/Shop Item List")]
public class ShopItemList : ScriptableObject 
{
    [SerializeField] private List<ShopInventoryItem> _items;
    [SerializeField] private float _buyMarkUp;

    public List<ShopInventoryItem> Items => _items;
    public float BuyMarkUp => _buyMarkUp;
}

[System.Serializable]
public struct ShopInventoryItem
{
    public InventoryItemData ItemData;
    public int Amount;
}
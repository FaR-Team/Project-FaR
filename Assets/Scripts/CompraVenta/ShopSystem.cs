using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ShopSystem
{
    [SerializeField] private List<ShopSlot> _shopInventory;
    [SerializeField] private float _buyMarkUp;

    public List<ShopSlot> ShopInventory => _shopInventory;
    public float BuyMarkUp => _buyMarkUp;


    public ShopSystem(int size, float buyMarkUp)
    {
        _buyMarkUp = buyMarkUp;

        SetShopSize(size);
    }

    private void SetShopSize(int size)
    {
        _shopInventory = new List<ShopSlot>(size);

        for (int i = 0; i < size; i++)
        {
            _shopInventory.Add(new ShopSlot());
        }

    }

    public void AddToShop(InventoryItemData data, int amount)
    {
        if (ContieneItem(data, out ShopSlot shopSlot))
        {
            shopSlot.AddToStack(amount);
        }

        var freeSlot = GetFreeSlot();
        freeSlot.AssignItem(data, amount);
    }

    private ShopSlot GetFreeSlot()
    {
        var freeSlot = _shopInventory.FirstOrDefault(i => i.ItemData == null);

        if (freeSlot == null)
        {
            freeSlot = new ShopSlot();
            _shopInventory.Add(freeSlot);
        }

        return freeSlot;
    }

    public bool ContieneItem(InventoryItemData itemAAñadir, out ShopSlot shopSlot) //alguno de los slots ya tiene este item?
    {
        shopSlot = _shopInventory.Find(i => i.ItemData == itemAAñadir);
        return shopSlot != null;
    }

    public void PurchaseItem(InventoryItemData data, int amount)
    {
        if (!ContieneItem(data, out ShopSlot slot)) return;
        
        slot.SacarDeStack(amount);
    }
}

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SellSystem : MonoBehaviour
{
    public int BoxCount = 0;
    public GameObject Box1, Box2, Box3, Box4, Box5, Box6;
    private TextMeshProUGUI Box1T, Box2T, Box3T, Box4T, Box5T;
    private int Box1int, Box2int, Box3int, Box4int, Box5int;
    public GameObject BigBox;

    [SerializeField] private PlayerInventoryHolder _playerInventoryHolder;
    [SerializeField] private int _basketTotal;

    [SerializeField] private ShoppingCartItemUI _shoppingCartItemPrefab;
    [SerializeField] private GameObject _shoppingCartContentPanel;
    [SerializeField] private GameObject _shoppingCartObj;

    private Dictionary<InventoryItemData, int> _shoppingCart = new Dictionary<InventoryItemData, int>();
    private Dictionary<InventoryItemData, ShoppingCartItemUI> _shoppingCartUI = new Dictionary<InventoryItemData, ShoppingCartItemUI>();

    public void SellItem(GameObject CropBoxPrefab, InventoryItemData data)
    {
        AddToSellCart(CropBoxPrefab, data);
    }

    public void Sell()
    {
        _playerInventoryHolder.PrimaryInventorySystem.GainGold(_basketTotal);
        ClearSlots();
    }
    
    public void AddToSellCart(GameObject CropBoxPrefab, InventoryItemData data)
    {
        var price = GetModifiedPrice(data, 1);

        if (_shoppingCart.ContainsKey(data))
        {
            _shoppingCart[data]++;

            if (data.ID == Box1int) Box1T.text = $"{_shoppingCart[data]}";
            if (data.ID == Box2int) Box2T.text = $"{_shoppingCart[data]}";
            if (data.ID == Box3int) Box3T.text = $"{_shoppingCart[data]}";
            if (data.ID == Box4int) Box4T.text = $"{_shoppingCart[data]}";
            if (data.ID == Box5int) Box5T.text = $"{_shoppingCart[data]}";
            var newString = $"{data.Nombre} x{_shoppingCart[data]}";
            _shoppingCartUI[data].SetItemText(newString);
        }
        else 
        {
            _shoppingCart.Add(data, 1);
            AddBox(CropBoxPrefab, data);
            var shoppingCartTextObj = Instantiate(_shoppingCartItemPrefab, _shoppingCartContentPanel.transform);
            var newString = $"{data.Nombre} x{_shoppingCart[data]}";
            _shoppingCartUI.Add(data, shoppingCartTextObj);
            _shoppingCartUI[data].SetItemText(newString);
        }

        _basketTotal += price;
    }

    public void AddBox(GameObject CropBoxPrefab, InventoryItemData data)
    {
        if (BoxCount == 0)
        {
            Vector3 dir = -transform.right;
            float offset = 2;
            Vector3 position = transform.position + dir * offset;
            var box1 = Instantiate(CropBoxPrefab, position, transform.rotation);
            Box1 = box1;
            Transform trans = Box1.transform;
            Box1T = trans.Find("default").gameObject.GetComponentInChildren<TextMeshProUGUI>();
            Box1T.text = $"{_shoppingCart[data]}";
            Box1int = data.ID;
            BoxCount = 1;
        }
        else if (BoxCount == 1)
        {
            Vector3 dir1 = transform.forward;
            float offset1 = 2;
            Vector3 position = transform.position + dir1 * offset1;
            var box2 = Instantiate(CropBoxPrefab, position, transform.rotation);
            Box2 = box2;
            Transform trans = Box2.transform;
            Box2T = trans.Find("default").gameObject.GetComponentInChildren<TextMeshProUGUI>();
            Box2T.text = $"{_shoppingCart[data]}";
            Box2int = data.ID;
            BoxCount = 2;
        }
        else if (BoxCount == 2)
        {
            Vector3 dir1 = -transform.forward;
            float offset1 = 2;
            Vector3 position = transform.position + dir1 * offset1;
            var box3 = Instantiate(CropBoxPrefab, position, transform.rotation);
            Box3 = box3;
            Transform trans = Box3.transform;
            Box3T = trans.Find("default").gameObject.GetComponentInChildren<TextMeshProUGUI>();
            Box3T.text = $"{_shoppingCart[data]}";
            Box3int = data.ID;
            BoxCount = 3;
        }
        else if (BoxCount == 3)
        {
            Vector3 dir1 = transform.forward;
            float offset1 = 2;
            Vector3 dir2 = -transform.right;
            float offset2 = 2;
            Vector3 position = transform.position + dir1 * offset1 + dir2 * offset2;
            var box4 = Instantiate(CropBoxPrefab, position, transform.rotation);
            Box4 = box4;
            Transform trans = Box4.transform;
            Box4T = trans.Find("default").gameObject.GetComponentInChildren<TextMeshProUGUI>();
            Box4T.text = $"{_shoppingCart[data]}";
            Box4int = data.ID;
            BoxCount = 4;
        }
        else if (BoxCount == 4)
        {
            Vector3 dir1 = -transform.forward;
            float offset1 = 2;
            Vector3 dir2 = -transform.right;
            float offset2 = 2;
            Vector3 position = transform.position + dir1 * offset1 + dir2 * offset2;
            var box5 = Instantiate(CropBoxPrefab, position, transform.rotation);
            Box5 = box5;
            Transform trans = Box5.transform;
            Box5T = trans.Find("default").gameObject.GetComponentInChildren<TextMeshProUGUI>();
            Box5T.text = $"{_shoppingCart[data]}";
            Box5int = data.ID;
            BoxCount = 5;
        }
        else if (BoxCount == 5)
        {
            Vector3 dir = transform.up;
            float offset = 1;
            BoxCount = 6;
            Destroy(Box1);
            Destroy(Box2);
            Destroy(Box3);
            Destroy(Box4);
            Destroy(Box5);
            Destroy(Box6);
            Vector3 position = transform.position + dir * offset;
            BigBox.gameObject.SetActive(true);
        }
    }

    public static int GetModifiedPrice(InventoryItemData data, int amount)
    {
        var baseValue = data.Valor * amount;

        return Mathf.RoundToInt(baseValue);
    }

    private void ClearSlots()
    {
        _shoppingCart = new Dictionary<InventoryItemData, int>();
        _shoppingCartUI = new Dictionary<InventoryItemData, ShoppingCartItemUI>();
        _basketTotal = 0;
        BoxCount = 0;
        if(Box1 != null) Destroy(Box1);
        if(Box2 != null) Destroy(Box2);
        if(Box3 != null) Destroy(Box3);
        if(Box4 != null) Destroy(Box4);
        if(Box5 != null) Destroy(Box5);
        BigBox.gameObject.SetActive(false);

        foreach (var item in _shoppingCartContentPanel.transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }
    }
}

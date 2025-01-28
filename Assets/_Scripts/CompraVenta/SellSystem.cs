using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Utils;
using FaRUtils.Systems.DateTime;

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

    //private Dictionary<InventoryItemData, int> _shoppingCart = new Dictionary<InventoryItemData, int>();
    private static List<ShoppingCartItem> _shoppingCart = new(); 
    private Dictionary<InventoryItemData, ShoppingCartItemUI> _shoppingCartUI = new Dictionary<InventoryItemData, ShoppingCartItemUI>();
    
    private static bool firstLoadDone = false;
    public static List<ShoppingCartItem> ShoppingCart => _shoppingCart;
    private void OnEnable()
    {
        SleepHandler.Instance.OnPlayerSleep += Sell;
        _playerInventoryHolder = GameObject.FindWithTag("Player").GetComponent<PlayerInventoryHolder>();
    }

    private void Start()
    {
        _shoppingCart.Clear();
        SellSystemData data;
        if (firstLoadDone)
        {
            data = LoadAllData.GetData<GameStateData>(true).SellSystemData;
        }
        else
        {
            data = LoadAllData.GetData<GameStateData>(false).SellSystemData;
            firstLoadDone = true;
        }
        
        if(data != null) 
        {
            Load(data.shoppingCart);
            CatchUpMissedSales();
        }
        else Debug.LogWarning("SellSystemData is null");
    }

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

        if (TryGetShoppingCartItem(data, out ShoppingCartItem item))
        {
            item.amount++;
            //_shoppingCart[data]++;

            if (data.ID == Box1int) Box1T.text = $"{item.amount}";
            if (data.ID == Box2int) Box2T.text = $"{item.amount}";
            if (data.ID == Box3int) Box3T.text = $"{item.amount}";
            if (data.ID == Box4int) Box4T.text = $"{item.amount}";
            if (data.ID == Box5int) Box5T.text = $"{item.amount}";
            var newString = $"{data.Nombre} x{item.amount}";
            _shoppingCartUI[data].SetItemText(newString);
        }
        else
        {
            ShoppingCartItem cartItem = new ShoppingCartItem(data, 1);
            _shoppingCart.Add(cartItem);
            this.Log("AddCart");
            AddBox(CropBoxPrefab, data);
            var shoppingCartTextObj = Instantiate(_shoppingCartItemPrefab, _shoppingCartContentPanel.transform);
            var newString = $"{data.Nombre} x{cartItem.amount}";
            _shoppingCartUI.Add(data, shoppingCartTextObj);
            _shoppingCartUI[data].SetItemText(newString);
        }

        _basketTotal += price;
    }

    private void CatchUpMissedSales()
    {
        var currentTime = TimeManager.DateTime;
        var lastSaveTime = TimeManager.Instance.GetLastTimeInScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        var currentDayCount = currentTime.Hour < 3 ? currentTime.TotalNumDays - 1 : currentTime.TotalNumDays;
        var lastDayCount = lastSaveTime.Hour < 3 ? lastSaveTime.TotalNumDays - 1 : lastSaveTime.TotalNumDays;
        var daysPassed = currentDayCount - lastDayCount;

        if (daysPassed > 0)
        {
            Sell();
        }
    }

    public void AddBox(GameObject CropBoxPrefab, InventoryItemData data)
    {
        this.Log("AddBox");
        TryGetShoppingCartItem(data, out ShoppingCartItem cartItem);

        if (BoxCount >= 5)
        {
            HandleBigBox();
            return;
        }

        var (position, boxNumber) = GetNextBoxPosition();
        var box = InstantiateBox(CropBoxPrefab, position, cartItem.amount);
        StoreBoxReference(box, boxNumber, data.ID);
        BoxCount++;
    }

    private (Vector3 position, int boxNumber) GetNextBoxPosition()
    {
        switch (BoxCount)
        {
            case 0:
                return (transform.position + (-transform.right * 2), 1);
            case 1:
                return (transform.position + (transform.forward * 2), 2);
            case 2:
                return (transform.position + (-transform.forward * 2), 3);
            case 3:
                return (transform.position + (transform.forward * 2) + (-transform.right * 2), 4);
            case 4:
                return (transform.position + (-transform.forward * 2) + (-transform.right * 2), 5);
            default:
                return (Vector3.zero, 0);
        }
    }

    private GameObject InstantiateBox(GameObject prefab, Vector3 position, int amount)
    {
        var box = Instantiate(prefab, position, transform.rotation);
        var textComponent = box.transform.Find("default").gameObject.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = $"{amount}";
        return box;
    }

    private void StoreBoxReference(GameObject box, int boxNumber, int dataId)
    {
        switch (boxNumber)
        {
            case 1:
                Box1 = box;
                Box1T = box.transform.Find("default").gameObject.GetComponentInChildren<TextMeshProUGUI>();
                Box1int = dataId;
                break;
            case 2:
                Box2 = box;
                Box2T = box.transform.Find("default").gameObject.GetComponentInChildren<TextMeshProUGUI>();
                Box2int = dataId;
                break;
            case 3:
                Box3 = box;
                Box3T = box.transform.Find("default").gameObject.GetComponentInChildren<TextMeshProUGUI>();
                Box3int = dataId;
                break;
            case 4:
                Box4 = box;
                Box4T = box.transform.Find("default").gameObject.GetComponentInChildren<TextMeshProUGUI>();
                Box4int = dataId;
                break;
            case 5:
                Box5 = box;
                Box5T = box.transform.Find("default").gameObject.GetComponentInChildren<TextMeshProUGUI>();
                Box5int = dataId;
                break;
        }
    }

    private void HandleBigBox()
    {
        BoxCount = 6;
        Destroy(Box1);
        Destroy(Box2);
        Destroy(Box3);
        Destroy(Box4);
        Destroy(Box5);
        Destroy(Box6);
        BigBox.gameObject.SetActive(true);
        BigBox.transform.position = transform.position + (transform.up * 1);
    }

    public static int GetModifiedPrice(InventoryItemData data, int amount)
    {
        var baseValue = data.Valor * amount;

        return Mathf.RoundToInt(baseValue);
    }

    private void ClearSlots()
    {
        _shoppingCart = new ();
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

    bool IsItemInShoppingCart(InventoryItemData data)
    {
        return _shoppingCart.Any(i => i.data == data);
    }

    bool TryGetShoppingCartItem(InventoryItemData data, out ShoppingCartItem item)
    {
        item = _shoppingCart.FirstOrDefault(i => i.data == data);
        return item != null;
    }

    public void Load(List<ShoppingCartItem> shoppingCart)
    {
        if (shoppingCart == null || shoppingCart.Count == 0)
        {
            Debug.LogWarning("Loaded shopping cart list is null or empty");
            return;
        }
        
        // Restore boxes state
        for (int i = 0; i < shoppingCart.Count; i++)
        {
            if(!(shoppingCart[i].data is CropItemData cropItemData)) continue;
            
            for (int j = 0; j < shoppingCart[i].amount; j++)
            {
                AddToSellCart(cropItemData.CropBoxPrefab, shoppingCart[i].data);
            }
            
        }
    }
    
    private void OnDisable()
    {
        SleepHandler.Instance.OnPlayerSleep -= Sell;
    }
}

[System.Serializable]
public class SellSystemData
{
    public List<ShoppingCartItem> shoppingCart = new();

    public SellSystemData()
    {
        shoppingCart = new List<ShoppingCartItem>();
    }

    public SellSystemData(List<ShoppingCartItem> shoppingCart)
    {
        this.shoppingCart = shoppingCart;
    }
}

[System.Serializable]
public class ShoppingCartItem
{
    public InventoryItemData data;
    public int amount;

    public ShoppingCartItem(InventoryItemData data, int amount)
    {
        this.data = data;
        this.amount = amount;
    }
}

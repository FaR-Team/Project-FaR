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
        
        if (BoxCount == 0)
        {
            Vector3 dir = -transform.right;
            float offset = 2;
            Vector3 position = transform.position + dir * offset;
            var box1 = Instantiate(CropBoxPrefab, position, transform.rotation);
            Box1 = box1;
            Transform trans = Box1.transform;
            Box1T = trans.Find("default").gameObject.GetComponentInChildren<TextMeshProUGUI>();
            Box1T.text = $"{cartItem.amount}";
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
            Box2T.text = $"{cartItem.amount}";
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
            Box3T.text = $"{cartItem.amount}";
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
            Box4T.text = $"{cartItem.amount}";
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
            Box5T.text = $"{cartItem.amount}";
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

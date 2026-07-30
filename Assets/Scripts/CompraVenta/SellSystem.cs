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

    public static SellSystem Instance { get; private set; }

    private List<ShoppingCartItem> _shoppingCart = new(); 
    private Dictionary<InventoryItemData, ShoppingCartItemUI> _shoppingCartUI = new Dictionary<InventoryItemData, ShoppingCartItemUI>();
    
    private static bool firstLoadDone = false;
    private static bool sellRequested = false;
    public List<ShoppingCartItem> ShoppingCart => _shoppingCart;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnEnable()
    {
        RegisterEvents();
        UpdatePlayerInventoryHolder();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        if (SleepHandler.Instance != null)
        {
            SleepHandler.Instance.OnPlayerSleep -= Sell;
            SleepHandler.Instance.OnPlayerSleep += Sell;
        }
        if (CatchUpBroadcaster.Instance != null)
        {
            CatchUpBroadcaster.Instance.OnCatchUpBroadcast -= wasSellRequested;
            CatchUpBroadcaster.Instance.OnCatchUpBroadcast += wasSellRequested;
        }
    }

    private void UnregisterEvents()
    {
        if (SleepHandler.Instance != null)
        {
            SleepHandler.Instance.OnPlayerSleep -= Sell;
        }
        if (CatchUpBroadcaster.Instance != null)
        {
            CatchUpBroadcaster.Instance.OnCatchUpBroadcast -= wasSellRequested;
        }
    }

    private void UpdatePlayerInventoryHolder()
    {
        if (_playerInventoryHolder == null)
        {
            if (PlayerInventoryHolder.instance != null)
            {
                _playerInventoryHolder = PlayerInventoryHolder.instance;
            }
            else
            {
                var playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                {
                    _playerInventoryHolder = playerObj.GetComponent<PlayerInventoryHolder>();
                }
            }
        }
    }

    private void Start()
    {
        RegisterEvents();
        UpdatePlayerInventoryHolder();

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
        }
        else Debug.LogWarning("SellSystemData is null");
    }

    public void SellItem(GameObject CropBoxPrefab, InventoryItemData data)
    {
        AddToSellCart(CropBoxPrefab, data);
    }

    public void Sell()
    {
        UpdatePlayerInventoryHolder();

        if (_playerInventoryHolder != null && _playerInventoryHolder.PrimaryInventorySystem != null)
        {
            _playerInventoryHolder.PrimaryInventorySystem.GainGold(_basketTotal);
            this.LogSuccess($"Sold for {_basketTotal} gold");
        }
        else
        {
            Debug.LogError("PlayerInventoryHolder or PrimaryInventorySystem is null in SellSystem.Sell()");
        }

        ClearSlots();
        sellRequested = false;
    }

    public static void ProcessPendingSale()
    {
        if (Instance != null)
        {
            if (Instance._shoppingCart.Count > 0 || Instance._basketTotal > 0)
            {
                Instance.Sell();
            }
            return;
        }

        GameStateData gameState = LoadAllData.GetData<GameStateData>(true);
        if (gameState != null && gameState.SellSystemData != null && gameState.SellSystemData.shoppingCart != null && gameState.SellSystemData.shoppingCart.Count > 0)
        {
            int totalGold = 0;
            foreach (var item in gameState.SellSystemData.shoppingCart)
            {
                if (item != null && item.data != null)
                {
                    totalGold += GetModifiedPrice(item.data, item.amount);
                }
            }

            var playerInv = PlayerInventoryHolder.instance != null 
                ? PlayerInventoryHolder.instance 
                : GameObject.FindWithTag("Player")?.GetComponent<PlayerInventoryHolder>();

            if (playerInv != null && playerInv.PrimaryInventorySystem != null)
            {
                playerInv.PrimaryInventorySystem.GainGold(totalGold);
                Debug.Log($"[SellSystem] Processed offline sleep sale: Granted {totalGold} gold to player.");
            }
            else
            {
                Debug.LogWarning("[SellSystem] Could not find PlayerInventoryHolder to grant gold for offline sale.");
            }

            gameState.SellSystemData.shoppingCart.Clear();
        }
    }
    
    public void AddToSellCart(GameObject CropBoxPrefab, InventoryItemData data)
    {
        if (data == null) return;
        var price = GetModifiedPrice(data, 1);

        if (TryGetShoppingCartItem(data, out ShoppingCartItem item))
        {
            item.amount++;

            if (data.ID == Box1int && Box1T != null) Box1T.text = $"{item.amount}";
            if (data.ID == Box2int && Box2T != null) Box2T.text = $"{item.amount}";
            if (data.ID == Box3int && Box3T != null) Box3T.text = $"{item.amount}";
            if (data.ID == Box4int && Box4T != null) Box4T.text = $"{item.amount}";
            if (data.ID == Box5int && Box5T != null) Box5T.text = $"{item.amount}";

            var newString = $"{data.Nombre} x{item.amount}";
            if (_shoppingCartUI.TryGetValue(data, out var uiItem) && uiItem != null)
            {
                uiItem.SetItemText(newString);
            }
            else if (_shoppingCartItemPrefab != null && _shoppingCartContentPanel != null)
            {
                var shoppingCartTextObj = Instantiate(_shoppingCartItemPrefab, _shoppingCartContentPanel.transform);
                _shoppingCartUI[data] = shoppingCartTextObj;
                shoppingCartTextObj.SetItemText(newString);
            }
        }
        else
        {
            ShoppingCartItem cartItem = new ShoppingCartItem(data, 1);
            _shoppingCart.Add(cartItem);
            AddBox(CropBoxPrefab, data);

            var newString = $"{data.Nombre} x{cartItem.amount}";
            if (_shoppingCartItemPrefab != null && _shoppingCartContentPanel != null)
            {
                var shoppingCartTextObj = Instantiate(_shoppingCartItemPrefab, _shoppingCartContentPanel.transform);
                _shoppingCartUI[data] = shoppingCartTextObj;
                shoppingCartTextObj.SetItemText(newString);
            }
        }

        _basketTotal += price;
    }

    private void wasSellRequested(int daysPassed)
    {
        if (daysPassed > 0)
        {
            sellRequested = true;
        }
        else
        {
            sellRequested = false;
        }
    }

    public void AddBox(GameObject CropBoxPrefab, InventoryItemData data)
    {
        int amount = 1;
        if (TryGetShoppingCartItem(data, out ShoppingCartItem cartItem) && cartItem != null)
        {
            amount = cartItem.amount;
        }

        if (BoxCount >= 5)
        {
            HandleBigBox();
            return;
        }

        var (position, boxNumber) = GetNextBoxPosition();
        var box = InstantiateBox(CropBoxPrefab, position, amount);
        if (box != null && data != null)
        {
            StoreBoxReference(box, boxNumber, data.ID);
            BoxCount++;
        }
    }

    private (Vector3 position, int boxNumber) GetNextBoxPosition()
    {
        switch (BoxCount)
        {
            case 0:
                return (transform.position + (-transform.forward * 2), 3);
            case 1:
                return (transform.position + (transform.forward * 2), 2);
            case 2:
                return (transform.position + (-transform.right * 2), 1);
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
        if (prefab == null)
        {
            Debug.LogWarning("CropBoxPrefab is null when instantiating box.");
            return null;
        }

        var box = Instantiate(prefab, position, transform.rotation);
        var textComponent = box.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = $"{amount}";
        }
        return box;
    }

    private void StoreBoxReference(GameObject box, int boxNumber, int dataId)
    {
        if (box == null) return;
        var textComp = box.GetComponentInChildren<TextMeshProUGUI>();
        switch (boxNumber)
        {
            case 1:
                Box1 = box;
                Box1T = textComp;
                Box1int = dataId;
                break;
            case 2:
                Box2 = box;
                Box2T = textComp;
                Box2int = dataId;
                break;
            case 3:
                Box3 = box;
                Box3T = textComp;
                Box3int = dataId;
                break;
            case 4:
                Box4 = box;
                Box4T = textComp;
                Box4int = dataId;
                break;
            case 5:
                Box5 = box;
                Box5T = textComp;
                Box5int = dataId;
                break;
        }
    }

    private void HandleBigBox()
    {
        BoxCount = 6;
        if (Box1 != null) Destroy(Box1);
        if (Box2 != null) Destroy(Box2);
        if (Box3 != null) Destroy(Box3);
        if (Box4 != null) Destroy(Box4);
        if (Box5 != null) Destroy(Box5);
        if (Box6 != null) Destroy(Box6);
        if (BigBox != null)
        {
            BigBox.gameObject.SetActive(true);
            BigBox.transform.position = transform.position + (transform.up * 1);
        }
    }

    public static int GetModifiedPrice(InventoryItemData data, int amount)
    {
        var baseValue = data.Valor * amount;

        return Mathf.RoundToInt(baseValue);
    }

    private void ClearSlots()
    {
        _shoppingCart.Clear();
        _shoppingCartUI.Clear();
        _basketTotal = 0;
        BoxCount = 0;
        if (Box1 != null) Destroy(Box1);
        if (Box2 != null) Destroy(Box2);
        if (Box3 != null) Destroy(Box3);
        if (Box4 != null) Destroy(Box4);
        if (Box5 != null) Destroy(Box5);
        if (BigBox != null) BigBox.gameObject.SetActive(false);

        if (_shoppingCartContentPanel != null)
        {
            foreach (var item in _shoppingCartContentPanel.transform.Cast<Transform>())
            {
                Destroy(item.gameObject);
            }
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
        ClearSlots();

        if (shoppingCart == null || shoppingCart.Count == 0)
        {
            return;
        }
        
        // Restore boxes state
        for (int i = 0; i < shoppingCart.Count; i++)
        {
            if (shoppingCart[i] == null || shoppingCart[i].data == null) continue;
            GameObject boxPrefab = (shoppingCart[i].data as CropItemData)?.CropBoxPrefab;
            
            for (int j = 0; j < shoppingCart[i].amount; j++)
            {
                AddToSellCart(boxPrefab, shoppingCart[i].data);
            }
        }

        if (sellRequested)
        {
            Sell();
        }
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

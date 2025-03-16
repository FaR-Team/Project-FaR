using FaRUtils.FPSController;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopKeeperDisplay : MonoBehaviour
{
    public static ShopKeeperDisplay Instance { get; private set; }

    [SerializeField] private ShopSlotUI _shopSlotPrefab;
    [SerializeField] private ShoppingCartItemUI _shoppingCartItemPrefab;

    [SerializeField] private Button _buyTab;
    [SerializeField] private Button _sellTab;

    [Header("Carrito de Compra")]
    [SerializeField] private TextMeshProUGUI _basketTotalText;
    [SerializeField] private TextMeshProUGUI _playerGoldText;
    [SerializeField] private TextMeshProUGUI _shopGoldText;
    [SerializeField] private Button _buyButton;
    [SerializeField] private TextMeshProUGUI _buyButtonText;

    [Header("Vista Previa de Items")]
    [SerializeField] private Image _itemPreviewSprite;
    [SerializeField] public Image _itemPreviewSprite2;
    [SerializeField] private TextMeshProUGUI _itemPreviewName;
    [SerializeField] private TextMeshProUGUI _itemPreviewDescription;

    [SerializeField] private GameObject _itemListContentPanel;
    [SerializeField] private GameObject _shoppingCartObj;
    [SerializeField] private GameObject _shoppingCartContentPanel;
    [SerializeField] private GameObject _shoppingUIParent;
    public GameObject player;

    private int _basketTotal;
    public int _BuyMulti = 1;

    private ShopSystem _shopSystem;
    private PlayerInventoryHolder _playerInventoryHolder;

    private Dictionary<InventoryItemData, int> _shoppingCart = new Dictionary<InventoryItemData, int>();
    private Dictionary<InventoryItemData, ShoppingCartItemUI> _shoppingCartUI = new Dictionary<InventoryItemData, ShoppingCartItemUI>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void DisplayShopWindow(ShopSystem shopSystem, PlayerInventoryHolder playerInventoryHolder)
    {
        _shopSystem = shopSystem;
        _playerInventoryHolder = playerInventoryHolder;

        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (_buyButton != null)
        {
            _buyButton.onClick.RemoveAllListeners();
            _buyButton.onClick.AddListener(BuyItems);
        }

        ClearSlots();
        ClearItemPreview();

        _basketTotalText.enabled = false;
        _buyButton.gameObject.SetActive(false);
        _basketTotal = 0;
        _playerGoldText.text = $"{_playerInventoryHolder.PrimaryInventorySystem.Gold}";

        DisplayShopInventory();
    }

    private void BuyItems()
    {
        if (_playerInventoryHolder.PrimaryInventorySystem.Gold < _basketTotal) return;

        if (!_playerInventoryHolder.PrimaryInventorySystem.CheckInventoryRemaining(_shoppingCart)) return;

        foreach (var kvp in _shoppingCart)
        {
            _shopSystem.PurchaseItem(kvp.Key, kvp.Value);

            for (int i = 0; i < kvp.Value; i++)
            {
                _playerInventoryHolder.PrimaryInventorySystem.AddToInventory(kvp.Key, 1);
            }
        }

        _playerInventoryHolder.PrimaryInventorySystem.SpendGold(_basketTotal);

        CloseShopUITab();
    }

    private void ClearSlots()
    {
        _shoppingCart = new Dictionary<InventoryItemData, int>();
        _shoppingCartUI = new Dictionary<InventoryItemData, ShoppingCartItemUI>();

        foreach (var item in _itemListContentPanel.transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }

        foreach (var item in _shoppingCartContentPanel.transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }
    }

    private void DisplayShopInventory()
    {
        foreach (var item in _shopSystem.ShopInventory)
        {
            if (item.ItemData == null) continue;

            var shopSlot = Instantiate(_shopSlotPrefab, _itemListContentPanel.transform);
            shopSlot.Init(item, _shopSystem.BuyMarkUp);
        }
    }


    public void AddItemToCart(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;

        UpdateItemPreview(shopSlotUI);

        var price = GetModifiedPrice(data, _BuyMulti, shopSlotUI.MarkUp);

        if (_shoppingCart.ContainsKey(data))
        {
            if (_BuyMulti == 1)
            {
                _shoppingCart[data]++;
            }
            else if (_BuyMulti == 5)
            {
                _shoppingCart[data]++;
                _shoppingCart[data]++;
                _shoppingCart[data]++;
                _shoppingCart[data]++;
                _shoppingCart[data]++;
            }
            else if (_BuyMulti == 10)
            {
                _shoppingCart[data]++;
                _shoppingCart[data]++;
                _shoppingCart[data]++;
                _shoppingCart[data]++;
                _shoppingCart[data]++;
                _shoppingCart[data]++;
                _shoppingCart[data]++;
                _shoppingCart[data]++;
                _shoppingCart[data]++;
                _shoppingCart[data]++;
            }
            _itemPreviewSprite2.GetComponent<Animation>().Play("ItemBoop");
            var newString = $"{data.Nombre} ({data.Valor}G) x{_shoppingCart[data]}";
            _shoppingCartUI[data].SetItemText(newString);
        }
        else
        {
            _shoppingCart.Add(data, _BuyMulti);

            var shoppingCartTextObj = Instantiate(_shoppingCartItemPrefab, _shoppingCartContentPanel.transform);
            _itemPreviewSprite2.GetComponent<Animation>().Play("ItemBoop");
            var newString = $"{data.Nombre} ({data.Valor}G) x{_BuyMulti}";
            shoppingCartTextObj.SetItemText(newString);
            _shoppingCartUI.Add(data, shoppingCartTextObj);
        }

        _basketTotal += price;
        _basketTotalText.text = $"Total: {_basketTotal}G";

        if (_basketTotal > 0 && !_basketTotalText.IsActive())
        {
            _basketTotalText.enabled = true;
            _buyButton.gameObject.SetActive(true);
        }

        CheckCartVsAvaliableGold();

    }

    public void RemoveItemFromCart(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;
        var price = GetModifiedPrice(data, _BuyMulti, shopSlotUI.MarkUp);

        if (_shoppingCart.ContainsKey(data))
        {
            if (_basketTotal - price < 0) return;

            if (_shoppingCart[data] >= _BuyMulti)
            {
                _shoppingCart[data] -= _BuyMulti;
                var newString = $"{data.Nombre} ({price}G) x{_shoppingCart[data]}";
                _shoppingCartUI[data].SetItemText(newString);

                if (_shoppingCart[data] <= 0)
                {
                    _shoppingCart.Remove(data);
                    var tempObj = _shoppingCartUI[data].gameObject;
                    _shoppingCartUI.Remove(data);
                    Destroy(tempObj);
                }

                _basketTotal -= price;
                _basketTotalText.text = $"Total: {_basketTotal}G";

                if (_basketTotal <= 0 && _basketTotalText.IsActive())
                {
                    _basketTotalText.enabled = false;
                    _buyButton.gameObject.SetActive(false);
                    ClearItemPreview();
                    return;
                }
            }
        }

        CheckCartVsAvaliableGold();
    }

    public void x1Button()
    {
        _BuyMulti = 1;
    }

    public void x5Button()
    {
        _BuyMulti = 5;
    }

    public void x10Button()
    {
        _BuyMulti = 10;
    }

    public void CartButton()
    {
        if (!_shoppingCartObj.activeInHierarchy)
        {
            _shoppingCartObj.SetActive(true);
        }
        else
        {
            _shoppingCartObj.SetActive(false);
        }
    }

    private void UpdateItemPreview(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;

        _itemPreviewSprite.sprite = data.Icono;
        _itemPreviewSprite.color = Color.white;
        _itemPreviewSprite2.sprite = data.Icono;
        _itemPreviewSprite2.color = Color.white;
        _itemPreviewName.text = data.Nombre;
        _itemPreviewDescription.text = $"- {data.Descripción}";
    }

    private void ClearItemPreview()
    {
        _itemPreviewSprite.sprite = null;
        _itemPreviewSprite.color = Color.clear;
        _itemPreviewSprite2.sprite = null;
        _itemPreviewSprite2.color = Color.clear;
        _itemPreviewName.text = "";
        _itemPreviewDescription.text = "";
    }

    public static int GetModifiedPrice(InventoryItemData data, int amount, float markUp)
    {
        var baseValue = data.Valor * amount;

        return Mathf.RoundToInt(baseValue + baseValue * markUp);
    }

    private void CheckCartVsAvaliableGold()
    {
        var goldToCheck = _playerInventoryHolder.PrimaryInventorySystem.Gold;
        _basketTotalText.color = _basketTotal > goldToCheck ? Color.red : Color.green;

        if (_playerInventoryHolder.PrimaryInventorySystem.CheckInventoryRemaining(_shoppingCart)) return;

        _basketTotalText.color = Color.red;
        _basketTotalText.text = $"No hay suficiente espacio en el inventario";
    }

    public void CloseShopUITab()
    {
        RefreshDisplay();
        player = GameObject.FindWithTag("Player");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.GetComponent<FaRCharacterController>().enabled = true;
        Time.timeScale = 1f;
        StartCoroutine(WaitJustSoTheInventoryDoesntOpenTwice());
    }

    private IEnumerator WaitJustSoTheInventoryDoesntOpenTwice()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerInventoryHolder.IsBuying = false;
        _shoppingUIParent.SetActive(false);
        ShopKeeper.Instance.IsBuying = false;
    }
}

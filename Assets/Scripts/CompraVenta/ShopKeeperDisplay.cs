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

    [Header("Multiplier Buttons")]
    [SerializeField] private Button _x1Button;
    [SerializeField] private Button _x5Button;
    [SerializeField] private Button _x10Button;
    
    [Header("Button Sprites")]
    [SerializeField] private Sprite _x1NormalSprite;
    [SerializeField] private Sprite _x1SelectedSprite;
    [SerializeField] private Sprite _x5NormalSprite;
    [SerializeField] private Sprite _x5SelectedSprite;
    [SerializeField] private Sprite _x10NormalSprite;
    [SerializeField] private Sprite _x10SelectedSprite;

    [SerializeField] private GameObject _itemListContentPanel;
    [SerializeField] private GameObject _shoppingCartObj;
    [SerializeField] private GameObject _shoppingCartContentPanel;
    [SerializeField] private GameObject _shoppingUIParent;
    [SerializeField] private ShopPagination _shopPagination;
    [SerializeField] private ShopListLayout _shopListLayout;
    public GameObject player;

    private int _basketTotal;
    public int _BuyMulti = 1;

    private ShopSystem _shopSystem;
    private PlayerInventoryHolder _playerInventoryHolder;

    private Dictionary<InventoryItemData, int> _shoppingCart = new Dictionary<InventoryItemData, int>();
    private Dictionary<InventoryItemData, ShoppingCartItemUI> _shoppingCartUI = new Dictionary<InventoryItemData, ShoppingCartItemUI>();
    private List<GameObject> _shopSlots = new List<GameObject>();
    
    public bool AllowPriceUpdates { get; private set; } = true;
    
    private Vector2 _originalTextPosition;
    private Coroutine _currentShakeCoroutine;
    
    private int _baseBuyMulti = 1;
    private bool _isShiftHeld = false;
    private bool _isControlHeld = false;

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

    private void Start()
    {
        InitializeMultiplierButtons();
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool controlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        bool modifiersChanged = false;
        if (_isShiftHeld != shiftPressed)
        {
            _isShiftHeld = shiftPressed;
            modifiersChanged = true;
        }
        if (_isControlHeld != controlPressed)
        {
            _isControlHeld = controlPressed;
            modifiersChanged = true;
        }

        if (modifiersChanged)
        {
            UpdateEffectiveMultiplier();
        }
    }

    public void DisplayShopWindow(ShopSystem shopSystem, PlayerInventoryHolder playerInventoryHolder)
    {
        _shopSystem = shopSystem;
        _playerInventoryHolder = playerInventoryHolder;

        AllowPriceUpdates = true;

        _baseBuyMulti = 1;
        _BuyMulti = 1;
        UpdateMultiplierButtonStates();

        if (_basketTotalText != null)
        {
            _originalTextPosition = _basketTotalText.rectTransform.anchoredPosition;
        }

        // Hide hotbar and clock UI when shop opens
        HideGameUI();

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
        if (_playerInventoryHolder.PrimaryInventorySystem.Gold < _basketTotal)
        {
            if (_currentShakeCoroutine != null)
            {
                StopCoroutine(_currentShakeCoroutine);
                _basketTotalText.rectTransform.anchoredPosition = _originalTextPosition;
            }
            
            _currentShakeCoroutine = StartCoroutine(ShakeText(_basketTotalText));
            return;
        }

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

        ClearShoppingCart();
        
        _playerGoldText.text = $"{_playerInventoryHolder.PrimaryInventorySystem.Gold}";
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
        
        _shopSlots.Clear();
    }

    private void DisplayShopInventory()
    {
        foreach (var item in _shopSystem.ShopInventory)
        {
            if (item.ItemData == null) continue;

            var shopSlot = Instantiate(_shopSlotPrefab, _itemListContentPanel.transform);
            shopSlot.Init(item, _shopSystem.BuyMarkUp);
            _shopSlots.Add(shopSlot.gameObject);
        }
        
        if (_shopListLayout != null)
        {
            _shopListLayout.RefreshLayout();
        }
        
        if (_shopPagination != null)
        {
            if (_shopListLayout != null)
            {
                _shopPagination.SetItemsPerPage(_shopListLayout.GetItemsPerPage());
            }
            
            _shopPagination.Initialize(_shopSlots);
        }
    }


    public void AddItemToCart(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;

        UpdateItemPreview(shopSlotUI);

        var price = GetModifiedPrice(data, _BuyMulti, shopSlotUI.MarkUp);

        if (_shoppingCart.ContainsKey(data))
        {
            _shoppingCart[data] += _BuyMulti;
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
        _basketTotalText.text = $"{_basketTotal}G";

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
                _basketTotalText.text = $"{_basketTotal}G";

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

    private void InitializeMultiplierButtons()
    {
        UpdateMultiplierButtonStates();
    }

    private void UpdateEffectiveMultiplier()
    {
        if (_isControlHeld)
        {
            _BuyMulti = 10;
        }
        else if (_isShiftHeld)
        {
            _BuyMulti = 5;
        }
        else
        {
            _BuyMulti = _baseBuyMulti;
        }

        UpdateMultiplierButtonStates();
    }

    private void UpdateMultiplierButtonStates()
    {
        if (_x1Button != null)
        {
            if (_BuyMulti == 1)
            {
                _x1Button.image.sprite = _x1SelectedSprite != null ? _x1SelectedSprite : _x1Button.image.sprite;
            }
            else
            {
                _x1Button.image.sprite = _x1NormalSprite != null ? _x1NormalSprite : _x1Button.image.sprite;
            }
        }
        
        if (_x5Button != null)
        {
            if (_BuyMulti == 5)
            {
                _x5Button.image.sprite = _x5SelectedSprite != null ? _x5SelectedSprite : _x5Button.image.sprite;
            }
            else
            {
                _x5Button.image.sprite = _x5NormalSprite != null ? _x5NormalSprite : _x5Button.image.sprite;
            }
        }
        
        if (_x10Button != null)
        {
            if (_BuyMulti == 10)
            {
                _x10Button.image.sprite = _x10SelectedSprite != null ? _x10SelectedSprite : _x10Button.image.sprite;
            }
            else
            {
                _x10Button.image.sprite = _x10NormalSprite != null ? _x10NormalSprite : _x10Button.image.sprite;
            }
        }
    }

    public void x1Button()
    {
        _baseBuyMulti = 1;
        UpdateEffectiveMultiplier();
    }

    public void x5Button()
    {
        _baseBuyMulti = 5;
        UpdateEffectiveMultiplier();
    }

    public void x10Button()
    {
        _baseBuyMulti = 10;
        UpdateEffectiveMultiplier();
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

    public void ClearShoppingCart()
    {
        _shoppingCart.Clear();
        
        foreach (var cartItem in _shoppingCartUI.Values)
        {
            if (cartItem != null && cartItem.gameObject != null)
            {
                Destroy(cartItem.gameObject);
            }
        }
        _shoppingCartUI.Clear();
        
        _basketTotal = 0;
        _basketTotalText.text = "0G";
        _basketTotalText.color = Color.green;
        
        _basketTotalText.enabled = false;
        _buyButton.gameObject.SetActive(false);
        
        ClearItemPreview();
        
        if (_shoppingCartObj.activeInHierarchy)
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
        
        _baseBuyMulti = 1;
        _BuyMulti = 1;
        UpdateMultiplierButtonStates();
        
        player = GameObject.FindWithTag("Player");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.GetComponent<FaRCharacterController>().enabled = true;
        Time.timeScale = 1f;
        StartCoroutine(WaitJustSoTheInventoryDoesntOpenTwice());
    }

    public void CloseShopUITabSilent()
    {
        AllowPriceUpdates = false;
        
        RefreshDisplay();
        
        _baseBuyMulti = 1;
        _BuyMulti = 1;
        
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
        
        // Show game UI after the shop UI has been properly deactivated
        ShowGameUI();
    }

    private IEnumerator ShakeText(TextMeshProUGUI textToShake)
    {
        if (textToShake == null) yield break;

        if (_originalTextPosition == Vector2.zero || textToShake.rectTransform.anchoredPosition == _originalTextPosition)
        {
            _originalTextPosition = textToShake.rectTransform.anchoredPosition;
        }
        
        float shakeDuration = 0.5f;
        float shakeIntensity = 3f;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
            
            textToShake.rectTransform.anchoredPosition = _originalTextPosition + new Vector2(offsetX, offsetY);
            
            elapsed += Time.deltaTime;
            
            shakeIntensity = Mathf.Lerp(3f, 0f, elapsed / shakeDuration);
            
            yield return null;
        }

        textToShake.rectTransform.anchoredPosition = _originalTextPosition;
        
        _currentShakeCoroutine = null;
    }

    private void HideGameUI()
    {
        // Hide hotbar UI visually but keep GameObject active for input handling
        var hotbarDisplay = FindObjectOfType<HotbarDisplay>();
        if (hotbarDisplay != null)
        {
            // Hide the visual components instead of the entire GameObject
            var canvasGroup = hotbarDisplay.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = hotbarDisplay.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        // Hide clock UI through UIController
        if (UIController.instance != null)
        {
            UIController.instance.clockUI.SetActive(false);
        }
    }

    private void ShowGameUI()
    {
        // Show hotbar UI visually
        var hotbarDisplay = FindObjectOfType<HotbarDisplay>();
        if (hotbarDisplay != null)
        {
            // Show the visual components
            var canvasGroup = hotbarDisplay.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        // Show clock UI through UIController
        if (UIController.instance != null)
        {
            UIController.instance.clockUI.SetActive(true);
        }
    }
}

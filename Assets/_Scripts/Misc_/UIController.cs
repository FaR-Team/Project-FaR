using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.FPSController;
using FaRUtils.Systems.DateTime;

public class UIController : MonoBehaviour
{
    public static UIController instance { get; private set; }
    [SerializeField] private ShopKeeperDisplay _shopKeeperDisplay;
    public GameObject clockUI;
    public GameObject nameDisplayer;

    public static bool isPlayerInventoryOpen;
    public bool isChestInventoryOpen = false;

    private void Awake()
    {
        _shopKeeperDisplay.gameObject.SetActive(false);

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        CloseInventory();
    }

    private void Update()
    {
        if(_shopKeeperDisplay.gameObject.activeSelf)
        {
            nameDisplayer.SetActive(false);
        }
        else
        {
            nameDisplayer.SetActive(true);
        }

        if (GameInput.playerInputActions.Player.Inventory.WasPressedThisFrame() && isChestInventoryOpen == true ||  
            GameInput.playerInputActions.Player.Pause.WasPressedThisFrame() && isChestInventoryOpen == true) 
        {
            CloseInventory();
        }
    }

    private void OnEnable()
    {
        ShopKeeper.OnShopWindowRequested += DisplayShopWindow;
        Container.OnDynamicInventoryDisplayRequested += OpenPlayerAndDynamicInventory;
        PlayerInventoryHolder.OnPlayerInventoryDisplayRequested += OpenPlayerInventory; 
    }

    void OnDisable()
    {
        ShopKeeper.OnShopWindowRequested -= DisplayShopWindow;
        Container.OnDynamicInventoryDisplayRequested -= OpenPlayerAndDynamicInventory;
        PlayerInventoryHolder.OnPlayerInventoryDisplayRequested -= OpenPlayerInventory;
    }

    public void OpenPlayerInventory(InventorySystem inventorySystem, int offset)
    {
        InventoryUIController.instance.DisplayPlayerInventory(inventorySystem, offset);
        isPlayerInventoryOpen = true;
        clockUI.SetActive(false);
        PauseMenu.Instance.Pause();
    }

    public void CloseInventory()
    {
        InventoryUIController.instance.CloseInventories();
        clockUI.SetActive(true);
        PauseMenu.Instance.Unpause();
        isChestInventoryOpen = false;
        StartCoroutine(WaitJustSoTheInventoryDoesntOpenTwice());
    }

    public void OpenPlayerAndDynamicInventory(InventorySystem inventorySystem, int offset)
    {
        PlayerInventoryHolder.instance.OpenInventory();
        InventoryUIController.instance.DisplayInventory(inventorySystem, offset);
        isChestInventoryOpen = true;
    }

    private void DisplayShopWindow(ShopSystem shopSystem, PlayerInventoryHolder playerInventory)
    {
        _shopKeeperDisplay.gameObject.SetActive(true);
        _shopKeeperDisplay.DisplayShopWindow(shopSystem, playerInventory);
    }

    private IEnumerator WaitJustSoTheInventoryDoesntOpenTwice()
    {
        yield return new WaitForSeconds(0.1f);
        isPlayerInventoryOpen = false;
    }
}

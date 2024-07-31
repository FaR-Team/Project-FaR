using UnityEngine;
using UnityEngine.Events;

public class PlayerInventoryHolder : Container
{

    public static UnityAction OnPlayerInventoryChanged;

    public static UnityAction<InventorySystem, int> OnPlayerInventoryDisplayRequested;

    public GameObject TimeManager;
    public GameObject reloj;
    public static bool isInventoryOpen;
    public static bool IsBuying;
    public DynamicInventoryDisplay playerBackpackPanel;
    public InventoryUIController inventoryUIController;

    public static PlayerInventoryHolder instance;

    protected void Awake()
    {
        instance = this;

        inventorySystem = InventoryLoader.Load(tamañoInventario, _gold);
    }
    private void Start()
    {
        OnPlayerInventoryDisplayRequested?.Invoke(inventorySystem, offset);
        playerBackpackPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (GameInput.playerInputActions.Player.Inventory.WasPressedThisFrame() &&
            !isInventoryOpen &&
            !PauseMenu.GameIsPaused &&
            !ShopIsBuying() &&
            !IngameDebugConsole.DebugLogManager.Instance.isOnConsole)
        {
            OpenInventory();
        }
        else if ((GameInput.playerInputActions.Player.Inventory.WasPressedThisFrame() ||
                GameInput.playerInputActions.Player.Pause.WasPressedThisFrame()) &&
                isInventoryOpen &&
                PauseMenu.GameIsPaused &&
                !ShopIsBuying() &&
                !IngameDebugConsole.DebugLogManager.Instance.isOnConsole)
        {
            CloseInventory();
        }
    }

    private bool ShopIsBuying()
    {
        if (ShopKeeper.Instance == null)
        {
            return false;
        }
        else
        {
            return ShopKeeper.Instance.IsBuying;
        }
    }

    public void OpenInventory()
    {
        OnPlayerInventoryDisplayRequested?.Invoke(inventorySystem, offset);
        reloj.gameObject.SetActive(false);
        PauseMenu.GameIsPaused = true;
        isInventoryOpen = true;
        PauseMenu.Instance.Pause();
    }

    public void CloseInventory()
    {
        playerBackpackPanel.gameObject.SetActive(false);
        reloj.gameObject.SetActive(true);
        PauseMenu.GameIsPaused = false;
        isInventoryOpen = false;
        PauseMenu.Instance.Unpause();
    }
    public bool AñadirAInventario(InventoryItemData data, int amount)
    {
        return (inventorySystem.AñadirAInventario(data, amount));
    }
}

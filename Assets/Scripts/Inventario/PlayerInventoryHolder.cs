using UnityEngine;
using UnityEngine.Events;

public class PlayerInventoryHolder : Container
{
    public static UnityAction OnPlayerInventoryChanged;

    public static UnityAction<InventorySystem, int> OnPlayerInventoryDisplayRequested;

    public GameObject TimeManager;
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
            !UIController.isPlayerInventoryOpen &&
            !PauseMenu.GameIsPaused &&
            !ShopIsBuying() &&
            !IngameDebugConsole.DebugLogManager.Instance.isOnConsole)
        {
            OpenInventory();
        }
        else if ((GameInput.playerInputActions.Player.Inventory.WasPressedThisFrame() ||
                GameInput.playerInputActions.Player.Pause.WasPressedThisFrame()) &&
                UIController.isPlayerInventoryOpen &&
                PauseMenu.GameIsPaused &&
                !ShopIsBuying() &&
                !IngameDebugConsole.DebugLogManager.Instance.isOnConsole)
        {
            UIController.instance.CloseInventory();
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
    }

    public bool AñadirAInventario(InventoryItemData data, int amount)
    {
        return (inventorySystem.AñadirAInventario(data, amount));
    }
}

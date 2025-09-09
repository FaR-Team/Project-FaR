using UnityEngine;
using UnityEngine.Events;

public class PlayerInventoryHolder : Container
{
    public static UnityAction OnPlayerInventoryChanged;

    public static UnityAction<InventorySystem, int> OnPlayerInventoryDisplayRequested;
    private int _startingGold = 1000;
    
    public static bool IsBuying;
    public DynamicInventoryDisplay playerBackpackPanel;
    public InventoryUIController inventoryUIController;
    [SerializeField] private Spear spear;
    
    public static PlayerInventoryHolder instance;

    protected void Awake()
    {
        if(instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        inventorySystem = InventoryLoader.Load(tamañoInventario, _startingGold, false);
    }

    private void Start()
    {
        OnPlayerInventoryDisplayRequested?.Invoke(inventorySystem, offset);
        playerBackpackPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        //TODO: QUITAR ESTO
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

    public bool AddToInventory(InventoryItemData data, int amount)
    {
        return (inventorySystem.AddToInventory(data, amount));
    }

    public Spear GetSpear() // TODO: Haría un GetTool y que se le pase un enum, para cuando esté la red, esta y la lanza sean "MinigameTools" o algo así
    {
        return spear;
    }
}

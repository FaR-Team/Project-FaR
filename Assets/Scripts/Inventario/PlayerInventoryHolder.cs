using UnityEngine;
using UnityEngine.Events;

public class PlayerInventoryHolder : Container
{

    public static UnityAction OnPlayerInventoryChanged;

    public static UnityAction<InventorySystem, int> OnPlayerInventoryDisplayRequested;

    public GameObject ShopKeeperObj;
    public ShopKeeper shopKeeper;
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
        inventorySystem = new InventorySystem(tamañoInventario, _gold);
    }
    private void Start() {

        ShopKeeperObj = GameObject.FindGameObjectWithTag("Shop");
        shopKeeper = ShopKeeperObj.GetComponent<ShopKeeper>();
        OnPlayerInventoryDisplayRequested?.Invoke(inventorySystem, offset);
        playerBackpackPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (GameInput.playerInputActions.Player.Inventory.WasPressedThisFrame() && 
            isInventoryOpen == false && 
            PauseMenu.GameIsPaused == false && 
            shopKeeper.IsBuying == false && 
            IngameDebugConsole.DebugLogManager.Instance.isOnConsole == false)
        {
            OpenInventory();
        }
        else if ((GameInput.playerInputActions.Player.Inventory.WasPressedThisFrame() ||
                GameInput.playerInputActions.Player.Pause.WasPressedThisFrame()) &&
                isInventoryOpen == true && 
                PauseMenu.GameIsPaused == true && 
                shopKeeper.IsBuying == false &&
                IngameDebugConsole.DebugLogManager.Instance.isOnConsole == false)
        {
            CloseInventory();
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

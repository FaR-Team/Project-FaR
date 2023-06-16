using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FaRUtils.Systems.DateTime;

public class PlayerInventoryHolder : InventoryHolder
{

    public static UnityAction OnPlayerInventoryChanged;

    public static UnityAction<InventorySystem, int> OnPlayerInventoryDisplayRequested;

    public GameObject player;
    public GameObject ShopKeeperObj;
    public ShopKeeper shopKeeper;
    public GameObject TimeManager;
    public GameObject reloj;
    public static bool isInventoryOpen;
    public static bool IsBuying;
    public DynamicInventoryDisplay playerBackpackPanel;
    public InventoryUIController inventoryUIController;

    private void Start() {
        SaveGameManager.data.playerInventory = new InventorySaveData(primaryInventorySystem);
        player = GameObject.FindGameObjectWithTag("Player");
        ShopKeeperObj = GameObject.FindGameObjectWithTag("Shop");
        shopKeeper = ShopKeeperObj.GetComponent<ShopKeeper>();
        OnPlayerInventoryDisplayRequested?.Invoke(primaryInventorySystem, offset);
        playerBackpackPanel.gameObject.SetActive(false);
    }


    protected override void LoadInventory(SaveData data)
    {
        //Va a checkear los datos guardados para el inventario de este cofre, y si exisren, los va a cargar
        if (data.playerInventory.InvSystem == null) return;
        
        //Va a cargar los items del inventario
        this.primaryInventorySystem = data.playerInventory.InvSystem;
        OnPlayerInventoryChanged?.Invoke();
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
        OnPlayerInventoryDisplayRequested?.Invoke(primaryInventorySystem, offset);
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
        return (primaryInventorySystem.AñadirAInventario(data, amount)); 
    }
}

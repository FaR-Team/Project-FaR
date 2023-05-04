using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Characters.FirstPerson;
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
    public bool isInventoryOpen;
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
        if (GameInput.Instance.playerInputActions.Player.Inventory.WasPressedThisFrame() && 
            isInventoryOpen == false && 
            PauseMenu.GameIsPaused == false && 
            shopKeeper.IsBuying == false)
        {
            OpenInventory();
        }
        else if (   GameInput.Instance.playerInputActions.Player.Inventory.WasPressedThisFrame() && 
                    isInventoryOpen == true && 
                    PauseMenu.GameIsPaused == true && 
                    shopKeeper.IsBuying == false)
        {
            CloseInventory();
        }
    }
    public void OpenInventory()
    {
        OnPlayerInventoryDisplayRequested?.Invoke(primaryInventorySystem, offset);
        reloj.gameObject.SetActive(false);
        player.GetComponent<CubePlacer>().enabled = false;
        PauseMenu.GameIsPaused = true;
        isInventoryOpen = true;
        PauseMenu.Instance.Pause();
        //PauseGame();
    }

    public void CloseInventory()
    {
        playerBackpackPanel.gameObject.SetActive(false);
        reloj.gameObject.SetActive(true);
        player.GetComponent<CubePlacer>().enabled = true;
        PauseMenu.GameIsPaused = false;
        isInventoryOpen = false;
        PauseMenu.Instance.Unpause();
        //ResumeGame();
    }
    public bool AñadirAInventario(InventoryItemData data, int amount)
    {
        return (primaryInventorySystem.AñadirAInventario(data, amount)); 
    }

    void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.GetComponent<FirstPersonController>().enabled = false;
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.GetComponent<FirstPersonController>().enabled = true;
        Time.timeScale = 1;
    }
}

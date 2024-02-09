using System.Collections;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    public DynamicInventoryDisplay inventoryPanel;
    public DynamicInventoryDisplay playerBackpackPanel;
    public GameObject player;
    public GameObject reloj;
    public bool isChestInventoryOpen = false;

    public static InventoryUIController instance;

    private void Awake() 
    {
        instance = this;

        inventoryPanel.gameObject.SetActive(false);
        playerBackpackPanel.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player = GameObject.FindWithTag("Player");
    }

    private void OnEnable()
    {
        Cofre.OnDynamicInventoryDisplayRequested += DisplayInventory;
        RecycleBin.OnDynamicInventoryDisplayRequested += DisplayInventory;
        PlayerInventoryHolder.OnPlayerInventoryDisplayRequested += DisplayPlayerInventory;
    }

    void OnDisable()
    {
        Cofre.OnDynamicInventoryDisplayRequested -= DisplayInventory;
        RecycleBin.OnDynamicInventoryDisplayRequested -= DisplayInventory;
        PlayerInventoryHolder.OnPlayerInventoryDisplayRequested -= DisplayPlayerInventory;
    }
    void Update()
    {
        if (GameInput.playerInputActions.Player.Inventory.WasPressedThisFrame() && isChestInventoryOpen == true ||  
            GameInput.playerInputActions.Player.Pause.WasPressedThisFrame() && isChestInventoryOpen == true) {
            inventoryPanel.gameObject.SetActive(false);
            //playerBackpackPanel.gameObject.SetActive(false);
            //player.GetComponent<PlayerInventoryHolder>().CloseInventory();
            isChestInventoryOpen = false;
            StartCoroutine(WaitJustSoTheInventoryDoesntOpenTwice());
        }
    }

    private IEnumerator WaitJustSoTheInventoryDoesntOpenTwice()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerInventoryHolder.isInventoryOpen = false;
    }

    public void DisplayInventory(InventorySystem invToDisplay, int offset)
    {
        inventoryPanel.gameObject.SetActive(true);
        inventoryPanel.RefreshDynamicInventory(invToDisplay, offset);
    }

    public void DisplayPlayerInventory(InventorySystem invToDisplay, int offset)
    {
        playerBackpackPanel.gameObject.SetActive(true);
        playerBackpackPanel.RefreshDynamicInventory(invToDisplay, offset);
    }
}

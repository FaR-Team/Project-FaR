using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class InventoryUIController : MonoBehaviour
{
    public DynamicInventoryDisplay inventoryPanel;
    public DynamicInventoryDisplay playerBackpackPanel;
    public GameObject player;
    public GameObject reloj;
    public bool isChestInventoryOpen = false;

    private void Awake() 
    {
        inventoryPanel.gameObject.SetActive(false);
        playerBackpackPanel.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player = GameObject.FindWithTag("Player");
    }

    private void OnEnable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
        PlayerInventoryHolder.OnPlayerInventoryDisplayRequested += DisplayPlayerInventory;
    }

    void OnDisable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
        PlayerInventoryHolder.OnPlayerInventoryDisplayRequested -= DisplayPlayerInventory;
    }
    void Update()
    {
        if (GameInput.Instance.playerInputActions.Player.Inventory.WasPressedThisFrame() && isChestInventoryOpen == true ||  
            GameInput.Instance.playerInputActions.Player.Pause.WasPressedThisFrame() && isChestInventoryOpen == true) {
            inventoryPanel.gameObject.SetActive(false);
            playerBackpackPanel.gameObject.SetActive(false);
            reloj.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            player.GetComponent<FirstPersonController>().enabled = true;
            player.GetComponent<CubePlacer>().enabled = true;
            Time.timeScale = 1;
            isChestInventoryOpen = false;
            StartCoroutine(WaitJustSoTheInventoryDoesntOpenTwice());
        }

        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    playerBackpackPanel.gameObject.SetActive(false);
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //    player.GetComponent<FirstPersonController>().enabled = true;
        //    Time.timeScale = 1;
        //}
    }

    private IEnumerator WaitJustSoTheInventoryDoesntOpenTwice()
    {
        yield return new WaitForSeconds(0.1f);
        player.GetComponent<PlayerInventoryHolder>().isInventoryOpen = false;
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

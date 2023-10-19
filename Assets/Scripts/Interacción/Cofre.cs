using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.FPSController;

[RequireComponent(typeof(UniqueID))]
public class Cofre : InventoryHolder, IInteractable
{
    [SerializeField] private GameObject _prompt;
    public GameObject player;
    public GameObject reloj;
    public GameObject inventoryUIController;
    public GameObject InteractionPrompt => _prompt;

    protected override void Awake()
    {
        base.Awake();
        _prompt = GameObject.FindGameObjectWithTag("HouseInteraction");
    }

    private void Start() 
    {
        var ChestSaveData = new InventorySaveData(primaryInventorySystem, transform.position, transform.rotation);

    }

    protected override void LoadInventory(SaveData data)
    {
        
    }

    public void Interact(Interactor interactor,  out bool interactSuccessful)
    {
        player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerInventoryHolder>().OpenInventory();
        //PlayerInventoryHolder.OnPlayerInventoryDisplayRequested?.Invoke(primaryInventorySystem, offset);
        OnDynamicInventoryDisplayRequested?.Invoke(primaryInventorySystem, 0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.GetComponent<FaRCharacterController>().enabled = false;
        PlayerInventoryHolder.isInventoryOpen = true;
        inventoryUIController.GetComponent<InventoryUIController>().isChestInventoryOpen = true;
        interactSuccessful = true;
    }

    public void InteractOut()
    {
        Debug.Log("InteractOut");
    }

    public void EndInteraction()
    {
        Debug.Log("Terminando Interacción con Cofre");
    }
}
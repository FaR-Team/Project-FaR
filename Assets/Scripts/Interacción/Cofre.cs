using UnityEngine;
using FaRUtils.FPSController;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(UniqueID))]
public class Cofre : Container, IInteractable
{
    [SerializeField] private GameObject _prompt;
    public GameObject player;
    public GameObject reloj;
    public GameObject inventoryUIController;
    public GameObject InteractionPrompt => _prompt;

    public static UnityAction<InventorySystem, int> OnDynamicInventoryDisplayRequested;


    protected void Awake()
    {
        _prompt = GameObject.FindGameObjectWithTag("HouseInteraction"); //MODIFICAR. FINDGO ES LENTO, SE PUEDE HACER UN SINGLETON U OTRA COSA.
    }

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerInventoryHolder>().OpenInventory();
        //PlayerInventoryHolder.OnPlayerInventoryDisplayRequested?.Invoke(primaryInventorySystem, offset);
        OnDynamicInventoryDisplayRequested?.Invoke(inventorySystem, 0);
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

    public void LoadData(ChestData data)
    {
        inventorySystem = new InventorySystem(data.inventorySystem);
        transform.position = data.position;
    }

}
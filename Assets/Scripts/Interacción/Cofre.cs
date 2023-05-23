using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

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
        SaveLoad.OnLoadGame += LoadInventory;
        _prompt = GameObject.FindGameObjectWithTag("HouseInteraction");
    }

    private void Start() 
    {
        var ChestSaveData = new InventorySaveData(primaryInventorySystem, transform.position, transform.rotation);

        SaveGameManager.data.chestDictionary.Add(GetComponent<UniqueID>().ID, ChestSaveData);
    }

    protected override void LoadInventory(SaveData data)
    {
        //Va a checkear los datos guardados para el inventario de este cofre, y si exisren, los va a cargar
        if (data.chestDictionary.TryGetValue(GetComponent<UniqueID>().ID, out InventorySaveData chestData))
        {
            //Va a cargar los items del inventario
            this.primaryInventorySystem = chestData.InvSystem;
            this.transform.position = chestData.Position;
            this.transform.rotation = chestData.Rotation;
        }
    }

    public void Interact(Interactor interactor,  out bool interactSuccessful)
    {
        player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerInventoryHolder>().OpenInventory();
        //PlayerInventoryHolder.OnPlayerInventoryDisplayRequested?.Invoke(primaryInventorySystem, offset);
        OnDynamicInventoryDisplayRequested?.Invoke(primaryInventorySystem, 0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.GetComponent<FirstPersonController>().enabled = false;
        player.GetComponent<PlayerInventoryHolder>().isInventoryOpen = true;
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
using UnityEngine;
using FaRUtils.FPSController;
using UnityEngine.Events;

[RequireComponent(typeof(UniqueID))]
public class RecycleBin : Container, IInteractable
{
    [SerializeField] private GameObject _prompt;
    public GameObject InteractionPrompt => _prompt;

    protected void Awake()
    {
        _prompt = GameObject.FindGameObjectWithTag("HouseInteraction"); //MODIFICAR. FINDGO ES LENTO, SE PUEDE HACER UN SINGLETON U OTRA COSA.
        //TODO: DISCUTIR ESTO, TIENE QUE HABER DISTINTOS INTERACTION UI PARA CADA OBJETO, SI ES QUE ES IMPORTANTE, SI NO SALDRÍA F PARA INTERACTUAR
    }

    private void Start()
    {
        Cama.Instance.isSleepingEvent.AddListener(DeleteObjects);
        inventorySystem = new InventorySystem(inventorySystem);
    }

    private void OnDisable()
    {
        Cama.Instance.isSleepingEvent.RemoveListener(DeleteObjects);
    }

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        PlayerInventoryHolder.instance.OpenInventory();
        //PlayerInventoryHolder.OnPlayerInventoryDisplayRequested?.Invoke(primaryInventorySystem, offset);
        OnDynamicInventoryDisplayRequested?.Invoke(inventorySystem, 0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        FaRCharacterController.instance.enabled = false;
        PlayerInventoryHolder.isInventoryOpen = true;
        InventoryUIController.instance.isChestInventoryOpen = true;
        interactSuccessful = true;
    }

    private void DeleteObjects()
    {
        inventorySystem.ClearInventory();
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
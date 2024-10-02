using UnityEngine;
using FaRUtils.FPSController;
using UnityEngine.Events;
using Utils;

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
        SleepHandler.Instance.OnPlayerSleep += DeleteObjects;
        inventorySystem = new InventorySystem(inventorySystem);
    }

    private void OnDisable()
    {
        SleepHandler.Instance.OnPlayerSleep += DeleteObjects;
    }

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        OnDynamicInventoryDisplayRequested?.Invoke(inventorySystem, 0);
        interactSuccessful = true;
    }

    private void DeleteObjects()
    {
        inventorySystem.ClearInventory();
    }

    public void InteractOut()
    {
        this.Log("InteractOut");
    }

    public void EndInteraction()
    {
        this.Log("Terminando Interacción con Cofre");
    }
}
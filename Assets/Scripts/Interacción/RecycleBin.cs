using UnityEngine;
using FaRUtils.FPSController;
using UnityEngine.Events;
using Utils;

[RequireComponent(typeof(UniqueID))]
public class RecycleBin : Container, IInteractable
{
    [SerializeField] private InteractionPromptUI _prompt;
    public InteractionPromptUI InteractionPrompt => _prompt;
    public Transform InteractionTarget => transform;

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
        if (_prompt != null)
        {
            _prompt.Close();
        }
    }
}
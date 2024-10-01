using UnityEngine;
using FaRUtils.FPSController;
using UnityEngine.Events;

[RequireComponent(typeof(UniqueID))]
public class Cofre : Container, IInteractable
{
    [SerializeField] private GameObject _prompt;
    public GameObject InteractionPrompt => _prompt;

    protected void Awake()
    {
        _prompt = GameObject.FindGameObjectWithTag("HouseInteraction"); //MODIFICAR. FINDGO ES LENTO, SE PUEDE HACER UN SINGLETON U OTRA COSA.
        //TODO: DISCUTIR ESTO, TIENE QUE HABER DISTINTOS INTERACTION UI PARA CADA OBJETO, SI ES QUE ES IMPORTANTE, SI NO SALDRÍA F PARA INTERACTUAR
    }

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        OnDynamicInventoryDisplayRequested?.Invoke(inventorySystem, 0);
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
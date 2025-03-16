using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using FaRUtils.FPSController;
using FaRUtils.Systems.DateTime;
using UnityEngine.Events;
using Utils;

public class Cama : MonoBehaviour, IInteractable
{
    public static Cama Instance;

    [SerializeField] private GameObject _prompt;

    //I DON'T KNOW SWAHILI, BUT I THINK THIS IS THE CORRECT WAY TO DO THIS (Last part was written by github copilot.)
    public GameObject InteractionPrompt => _prompt;

    
    private void Awake() {
        _prompt = GameObject.FindGameObjectWithTag("BedInteraction");

        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    
    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        interactSuccessful = TrySleep();
    }

    public void InteractOut()
    {
        // Movido a TrySleep
    }
    
    public bool TrySleep()
    {
        this.Log("Interactuando con Cama");
        return SleepHandler.Instance.TrySleep();
    }
    
    public void EndInteraction()
    {
        this.Log("Terminando interacción con Cama");
    }
}

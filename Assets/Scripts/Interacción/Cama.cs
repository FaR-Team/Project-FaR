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

    [SerializeField] private InteractionPromptUI _prompt;

    public InteractionPromptUI InteractionPrompt => _prompt;
    public Transform InteractionTarget => transform;

    
    private void Awake() {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    
    public void Interact(InteractorBase interactor, out bool interactSuccessful)
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
        if (_prompt != null)
        {
            _prompt.Close();
        }
    }
}

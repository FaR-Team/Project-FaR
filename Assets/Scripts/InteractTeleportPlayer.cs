using System.Collections;
using System.Collections.Generic;
using FaRUtils.FPSController;
using UnityEngine;

public class InteractTeleportPlayer : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform newPosition;
    [SerializeField] private InteractionPromptUI _prompt;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public InteractionPromptUI InteractionPrompt => _prompt;
    public Transform InteractionTarget => transform;
    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        interactor.gameObject.GetComponent<FaRCharacterController>().Teleport(newPosition);
        interactSuccessful = true;
    }

    public void InteractOut()
    {
        throw new System.NotImplementedException();
    }

    public void EndInteraction()
    {
        if (InteractionPrompt != null)
        {
            InteractionPrompt.Close();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingInteractCollider : MonoBehaviour, IInteractable
{
    public InteractionPromptUI InteractionPrompt => null;
    public Transform InteractionTarget => transform;

    private FishingSpot _spot;

    public void Setup(FishingSpot spot)
    {
        _spot = spot;
    }
    public void Interact(InteractorBase interactor, out bool interactSuccessful)
    {
        interactSuccessful = true;
    }

    public void InteractOut()
    {
        //throw new System.NotImplementedException();
    }

    public void EndInteraction()
    {
        //throw new System.NotImplementedException();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTarget : MonoBehaviour, IInteractable
{
    [SerializeField] Collider col;
    private FishingSpot _spot;

    public void Setup(FishingSpot spot)
    {
        _spot = spot;
    }
    public InteractionPromptUI InteractionPrompt => null;
    public Transform InteractionTarget => transform;
    public void Interact(InteractorBase interactor, out bool interactSuccessful)
    {
        _spot.CaughtFish();
        interactSuccessful = true;
    }

    public void InteractOut()
    {
    }

    public void EndInteraction() // TODO: Ni se llama desde ThirdPersonInteractor
    {
    }

    public void EnableInteraction(bool enable)
    {
        col.enabled = enable;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    public GameObject InteractionPrompt { get; }

    public void Interact(Interactor interactor, out bool interactSuccessful);

    public void InteractOut();

    public void EndInteraction();
}

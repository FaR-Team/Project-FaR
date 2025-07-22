using UnityEngine;

public interface IInteractable
{
    public InteractionPromptUI InteractionPrompt { get; }
    public Transform InteractionTarget { get; }

    public void Interact(Interactor interactor, out bool interactSuccessful);

    public void InteractOut();

    public void EndInteraction();
}

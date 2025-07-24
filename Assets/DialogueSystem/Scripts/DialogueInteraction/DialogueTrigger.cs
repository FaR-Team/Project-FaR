using System.Collections;
using System.Collections.Generic;
using DS.ScriptableObjects;
using UnityEngine;

public class DialogueTrigger :  MonoBehaviour, IInteractable
{
    private InteractionPromptUI _prompt;

    public InteractionPromptUI InteractionPrompt => _prompt;
    public Transform InteractionTarget => transform;

    public DSDialogueContainerSO dialogueContainerSO;

    public string NPC_name;

    public void EndInteraction()
    {
        if (_prompt != null)
        {
            _prompt.Close();
        }
    }

    public void Interact(InteractorBase interactor, out bool interactSuccessful)
    {
        interactSuccessful = true;
        InteractOut();
    }

    public void InteractOut()
    {
        DialogueManager.instance.StartDialogueSequence(dialogueContainerSO, NPC_name);
    }
}

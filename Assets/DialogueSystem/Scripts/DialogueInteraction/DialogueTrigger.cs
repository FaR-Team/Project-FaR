using System.Collections;
using System.Collections.Generic;
using DS.ScriptableObjects;
using UnityEngine;

public class DialogueTrigger :  MonoBehaviour, IInteractable
{
    private GameObject _prompt;

    public GameObject InteractionPrompt => _prompt;

    public DSDialogueContainerSO dialogueContainerSO;

    public string NPC_name;

    public void EndInteraction()
    {
        throw new System.NotImplementedException();
    }

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        interactSuccessful = true;
        InteractOut();
    }

    public void InteractOut()
    {
        DiscordController.instance.UpdateDiscordRP("Talking to " + NPC_name);
        DialogueManager.instance.StartDialogueSequence(dialogueContainerSO, NPC_name);
    }
}

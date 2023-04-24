using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    private GameObject _prompt;

    public GameObject InteractionPrompt => _prompt;

    public TextMeshProUGUI nombreObj;
    public string nombre;

    [SerializeField] private DialogueObject dialogueObject;


    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        {
            if (responseEvents.DialogueObject == dialogueObject)
            {
                DialogueUI.Instance.AddResponseEvents(responseEvents.Events);
                break;
            }
        }

        nombreObj.GetComponent<TMPro.TextMeshProUGUI>().text = nombre.ToString();
        DialogueUI.Instance.ShowDialogue(dialogueObject);
        
        interactSuccessful = true;
    }

    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }

    public void EndInteraction()
    {
        //Debug.Log("Terminando interacción con NPC");
    }
}

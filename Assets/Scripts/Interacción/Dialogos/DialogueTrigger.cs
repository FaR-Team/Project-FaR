using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _prompt;

    public GameObject InteractionPrompt => _prompt;

    public TextMeshProUGUI nombreObj;
    public string nombre;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        nombreObj.GetComponent<TMPro.TextMeshProUGUI>().text = nombre.ToString();
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
        interactSuccessful = true;
    }

    public void EndInteraction()
    {
        Debug.Log("Terminando interacción con NPC");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DS.Data;
using DS.Enumerations;
using DS.ScriptableObjects;
using FaRUtils.FPSController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { get; private set; }

    
    public bool IsOpen { get; private set; }

    private GameObject player;
    public GameObject dialoguePanel;
    public GameObject dialogueChoicesContainer;
    public GameObject choicesPrefab;
    public TextMeshProUGUI dialogueName;
    public TextMeshProUGUI dialogueText;
    private TypewriterEffect typewriterEffect;


    void Awake()
    {
        if (instance != null && instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            instance = this; 
        } 
    }

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        player = GameObject.FindGameObjectWithTag("Player");

        CloseDialogueBox();
    }

    public void StartDialogueSequence(DSDialogueContainerSO dialogueContainerSO, string NPCname)
    {
        IsOpen = true;
        dialoguePanel.SetActive(true);
        player.GetComponent<FaRCharacterController>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DSDialogueSO firstDialogue = dialogueContainerSO.UngroupedDialogues.Find(x => x.IsStartingDialogue);
        dialogueName.text = NPCname;
        StartCoroutine(StepThroughDialogue(firstDialogue));
    }

    public void CloseDialogueBox()
    {
        IsOpen = false;
        player.GetComponent<FaRCharacterController>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        dialogueText.text = string.Empty;
        dialoguePanel.SetActive(false);
    }

    private IEnumerator StepThroughDialogue(DSDialogueSO dialogueObject)
    {
        dialogueText.text = ""; // Vaciar el texto antes de empezar

        yield return RunTypingEffect(dialogueObject.Text.ToString()); // Convertirlo a string y ejecutar RunTypingEffect

        if (dialogueObject.DialogueType ==  DSDialogueType.MultipleChoice)
        {
            yield return new WaitWhile(() => typewriterEffect.isRunning);
            ShowResponses(dialogueObject.Choices);
        }
        else if (dialogueObject.DialogueType ==  DSDialogueType.SingleChoice)
        {
            yield return new WaitUntil(() => Input.anyKey);
            yield return new WaitWhile(() => typewriterEffect.isRunning);
            if (dialogueObject.Choices[0].NextDialogue != null)
            {
                StartCoroutine(StepThroughDialogue(dialogueObject.Choices[0].NextDialogue));
            } else {
                CloseDialogueBox();
            }   
        }
    }

    private void ShowResponses(List<DSDialogueChoiceData> choices)
    {
        foreach (DSDialogueChoiceData choice in choices) 
        {
            GameObject _choice = Instantiate(choicesPrefab, dialogueChoicesContainer.transform);
            var choiceText = _choice.GetComponentInChildren<TextMeshProUGUI>();
            choiceText.text = choice.Text;
            _choice.GetComponent<Button>().onClick.AddListener(() => selectedChoice(choice.NextDialogue));
        }
    }

    private void selectedChoice(DSDialogueSO nextDialogue)
    {
        foreach (Transform child in dialogueChoicesContainer.transform)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(StepThroughDialogue(nextDialogue));
    }

    private IEnumerator RunTypingEffect(string dialogue)
    {
        typewriterEffect.Run(dialogue, dialogueText);

        while (typewriterEffect.isRunning)
        {
            yield return new WaitForSeconds(0.1f);

            yield return new WaitUntil(() => Input.anyKey);
            typewriterEffect.Stop();
            dialogueText.text = dialogue;
        }
    }
}

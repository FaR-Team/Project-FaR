using System.Collections;
using System.Collections.Generic;
using DS.ScriptableObjects;
using FaRUtils.FPSController;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { get; private set; }

    
    public bool IsOpen { get; private set; }

    private GameObject player;

    public DSDialogueSO startingDialogue;
    public GameObject dialoguePanel;
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
        dialoguePanel.SetActive(false);
        player.GetComponent<FaRCharacterController>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        dialogueText.text = string.Empty;
    }

    private IEnumerator StepThroughDialogue(DSDialogueSO dialogueObject)
{
    for (int i = 0; i < dialogueObject.Text.Length; i++)
    {
        char dialogueChar = dialogueObject.Text[i];

        yield return RunTypingEffect(dialogueChar.ToString());

        dialogueText.text += dialogueChar;

        if (i == dialogueObject.Text.Length - 1 && dialogueObject.HasChoices) break;

        yield return new WaitForSeconds(0.05f);
        yield return new WaitUntil(() => Input.anyKey);
    }

    if (dialogueObject.HasChoices)
    {
        // responseHandler.ShowResponses(dialogueObject.Choices);
    }
    else
    {
        CloseDialogueBox();
    }
}

    private IEnumerator RunTypingEffect(string dialogue)
    {
        typewriterEffect.Run(dialogue, dialogueText);

        while (typewriterEffect.isRunning)
        {
            yield return new WaitForSeconds(0.05f);

            if (Input.anyKey)
            {
                typewriterEffect.Stop();
            }
        }
    }
}

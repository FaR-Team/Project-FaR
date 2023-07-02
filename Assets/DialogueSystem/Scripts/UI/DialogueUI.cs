using System.Collections;
using System.Collections.Generic;
using DS.ScriptableObjects;
using FaRUtils.FPSController;
using UnityEngine;
using TMPro;

namespace DS.UI
{
    public class DialogueUI : MonoBehaviour
    {
        public static DialogueUI Instance;

        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private TMP_Text textLabel;

        public bool IsOpen { get; private set; }

        private GameObject player;

        //private ResponseHandler responseHandler;
        private TypewriterEffect typewriterEffect;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Start()
        {
            typewriterEffect = GetComponent<TypewriterEffect>();
            //responseHandler = GetComponent<ResponseHandler>();

            CloseDialogueBox();
        }

        public void ShowDialogue(DSDialogueSO dialogueObject)
        {
            IsOpen = true;
            dialogueBox.SetActive(true);
            player.GetComponent<FaRCharacterController>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //StartCoroutine(StepThroughDialogue(dialogueObject));
            textLabel.text = dialogueObject.Text;
        }

        /*
        public void AddResponseEvents(ResponseEvent[] responseEvents)
        {
            responseHandler.AddResponseEvents(responseEvents);
        }
        */

        private IEnumerator StepThroughDialogue(DSDialogueSO dialogueObject)
        {
            for (int i = 0; i < dialogueObject.Text.Length; i++)
            {
                string dialogue = dialogueObject.Text;

                yield return RunTypingEffect(dialogue);

                textLabel.text = dialogue;

                if (i == dialogueObject.Text.Length - 1 && dialogueObject.Choices != null) break;

                yield return new WaitForSeconds(0.05f);
                yield return new WaitUntil(() => Input.anyKey);
            }

            /*
            if (dialogueObject.Choices != null)
            {
                responseHandler.ShowResponses(dialogueObject.Responses);
            }
            else
            {
                CloseDialogueBox();
            }
            */
        }

        private IEnumerator RunTypingEffect(string dialogue)
        {
            typewriterEffect.Run(dialogue, textLabel);

            while (typewriterEffect.isRunning)
            {
                yield return new WaitForSeconds(0.05f);

                if (Input.anyKey)
                {
                    typewriterEffect.Stop();
                }
            }
        }

        public void CloseDialogueBox()
        {
            IsOpen = false;
            dialogueBox.SetActive(false);
            player.GetComponent<FaRCharacterController>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            textLabel.text = string.Empty;
        }
    }
}
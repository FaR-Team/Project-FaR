using System.Collections;
using System.Collections.Generic;
using DS.UI;
using UnityEngine;
using TMPro;

namespace DS
{
    [RequireComponent(typeof(DSDialogue))]
    public class DSDialogueTrigger : MonoBehaviour, IInteractable
    {
        private GameObject _prompt;
        public GameObject InteractionPrompt => _prompt;

        public TextMeshProUGUI nameObj;
        public string NPCname;

        public void Interact(Interactor interactor, out bool interactSuccessful)
        {
            nameObj.GetComponent<TMPro.TextMeshProUGUI>().text = NPCname.ToString();
            //DialogueUI.Instance.ShowDialogue(dialogue);

            interactSuccessful = true;
        }

        public void InteractOut()
        {
            throw new System.NotImplementedException();
        }

        public void EndInteraction()
        {
            throw new System.NotImplementedException();
        }
    }
}

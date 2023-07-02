using System;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Data
{
    using ScriptableObjects;
    
    [Serializable]
    public class DSDialogueChoiceData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public DSDialogueSO NextDialogue { get; set; }
    }
}

using System.Collections.Generic;
using UnityEditor;

namespace DS.Inspectors
{
    using Utilities;
    using ScriptableObjects;

    [CustomEditor(typeof(DSDialogue))]
    public class DSInspector : Editor
    {
        /* Scriptable Objects de Diálogos */
        private SerializedProperty dialogueContainerProperty;
        private SerializedProperty dialogueGroupProperty;
        private SerializedProperty dialogueProperty;

        /* Filtros */
        private SerializedProperty groupedDialoguesProperty;
        private SerializedProperty startingDialoguesOnlyProperty;

        /* Indexs */
        private SerializedProperty selectedDialogueGroupIndexProperty;
        private SerializedProperty selectedDialogueIndexProperty;

        private void OnEnable()
        {
            dialogueContainerProperty = serializedObject.FindProperty("dialogueContainer");
            dialogueGroupProperty = serializedObject.FindProperty("dialogueGroup");
            dialogueProperty = serializedObject.FindProperty("dialogue");
            
            groupedDialoguesProperty = serializedObject.FindProperty("groupedDialogues");
            startingDialoguesOnlyProperty = serializedObject.FindProperty("startingDialoguesOnly");
            
            selectedDialogueGroupIndexProperty = serializedObject.FindProperty("selectedDialogueGroupIndex");
            selectedDialogueIndexProperty = serializedObject.FindProperty("selectedDialogueIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawDialogueContainerArea();

            DSDialogueContainerSO dialogueContainer = (DSDialogueContainerSO) dialogueContainerProperty.objectReferenceValue;
            
            if (dialogueContainerProperty.objectReferenceValue == null)
            {
                StopDrawing("Seleccioná un contenedor de diálogos para ver el resto del inspector.");

                return;
            }

            DrawFiltersArea();
            
            bool currentStartingDialoguesOnlyFilter = startingDialoguesOnlyProperty.boolValue;

            List<string> dialogueNames;
            
            string dialogueFolderPath = $"Assets/DialogueSystem/Dialogues/{dialogueContainer.FileName}";

            string dialogueInfoMessage;
            
            if (groupedDialoguesProperty.boolValue)
            {
                List<string> dialogueGroupNames = dialogueContainer.GetDialogueGroupNames();

                if (dialogueGroupNames.Count == 0)
                {
                    StopDrawing("No hay grupos de diálogos en este contenedor.");

                    return;
                }

                DrawDialogueGroupArea(dialogueContainer, dialogueGroupNames);
                
                DSDialogueGroupSO dialogueGroup = (DSDialogueGroupSO) dialogueGroupProperty.objectReferenceValue;

                dialogueNames = dialogueContainer.GetGroupedDialogueNames(dialogueGroup, currentStartingDialoguesOnlyFilter);
                
                dialogueFolderPath += $"/Groups/{dialogueGroup.GroupName}/Dialogues";
                
                dialogueInfoMessage = $"No hay" + (currentStartingDialoguesOnlyFilter ? " diálogos de inicio" : " diálogos") + " en este grupo.";
            }
            else
            {
                dialogueNames = dialogueContainer.GetUngroupedDialogueNames(currentStartingDialoguesOnlyFilter);
                
                dialogueFolderPath += "/Global/Dialogues";
                
                dialogueInfoMessage = $"No hay" + (currentStartingDialoguesOnlyFilter ? " diálogos de inicio" : " diálogos") + " sin grupo en este contenedor.";
            }

            if (dialogueNames.Count == 0)
            {
                StopDrawing(dialogueInfoMessage);
                
                return;
            }

            DrawDialogueArea(dialogueNames, dialogueFolderPath);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDialogueContainerArea()
        {
            DSInspectorUtility.DrawHeader("Contenedor de Diálogos");

            dialogueContainerProperty.DrawPropertyField();
            
            DSInspectorUtility.DrawSpace();
        }
        
        private void DrawFiltersArea()
        {
            DSInspectorUtility.DrawHeader("Filtros");

            groupedDialoguesProperty.DrawPropertyField();
            startingDialoguesOnlyProperty.DrawPropertyField();
            
            DSInspectorUtility.DrawSpace();
        }
        
        private void DrawDialogueGroupArea(DSDialogueContainerSO dialogueContainer, List<string> dialogueGroupNames)
        {
            DSInspectorUtility.DrawHeader("Grupo de Diálogos");
            
            int oldSelectedDialogueGroupIndex = selectedDialogueGroupIndexProperty.intValue;
            
            DSDialogueGroupSO oldDialogueGroup = (DSDialogueGroupSO) dialogueGroupProperty.objectReferenceValue;

            bool isOldDialogueGroupNull = oldDialogueGroup == null;
            
            string oldDialogueGroupName = isOldDialogueGroupNull ? "" : oldDialogueGroup.GroupName;
            
            UpdateIndexOnNamesListUpdate(dialogueGroupNames, selectedDialogueGroupIndexProperty, oldSelectedDialogueGroupIndex, oldDialogueGroupName, isOldDialogueGroupNull);

            selectedDialogueGroupIndexProperty.intValue = DSInspectorUtility.DrawPopup("Grupo de Diálogos", selectedDialogueGroupIndexProperty.intValue, dialogueGroupNames.ToArray());

            string selectedDialogueGroupName = dialogueGroupNames[selectedDialogueGroupIndexProperty.intValue];
            
            DSDialogueGroupSO selectedDialogueGroup = DSIOUtility.LoadAsset<DSDialogueGroupSO>($"Assets/DialogueSystem/Dialogues/{dialogueContainer.FileName}/Groups/{selectedDialogueGroupName}", selectedDialogueGroupName);
            
            dialogueGroupProperty.objectReferenceValue = selectedDialogueGroup;
            
            DSInspectorUtility.DrawDisabledFields(() => dialogueGroupProperty.DrawPropertyField());
            
            DSInspectorUtility.DrawSpace();
        }

        private void DrawDialogueArea(List<string> dialogueNames, string dialogueFolderPath)
        {
            DSInspectorUtility.DrawHeader("Diálogos");

            int oldSelectedDialogueIndex = selectedDialogueIndexProperty.intValue;
            
            DSDialogueSO oldDialogue = (DSDialogueSO) dialogueProperty.objectReferenceValue;
            
            bool isOldDialogueNull = oldDialogue == null;
            
            string oldDialogueName = isOldDialogueNull ? "" : oldDialogue.DialogueName;
            
            UpdateIndexOnNamesListUpdate(dialogueNames, selectedDialogueIndexProperty, oldSelectedDialogueIndex, oldDialogueName, isOldDialogueNull);

            selectedDialogueIndexProperty.intValue = DSInspectorUtility.DrawPopup("Diálogos", selectedDialogueIndexProperty.intValue, dialogueNames.ToArray());
            
            string selectedDialogueName = dialogueNames[selectedDialogueIndexProperty.intValue];
            
            DSDialogueSO selectedDialogue = DSIOUtility.LoadAsset<DSDialogueSO>(dialogueFolderPath, selectedDialogueName);
            
            dialogueProperty.objectReferenceValue = selectedDialogue;

            DSInspectorUtility.DrawDisabledFields(() => dialogueProperty.DrawPropertyField());
        }
        
        private void StopDrawing(string reason, MessageType messageType = MessageType.Info)
        {
            DSInspectorUtility.DrawHelpBox(reason, messageType);
            
            DSInspectorUtility.DrawSpace();
            
            DSInspectorUtility.DrawHelpBox("Tenés que seleccionar un Diálogo para que este componente funcione correctamente.", MessageType.Warning);

            serializedObject.ApplyModifiedProperties();
        }
        
        private void UpdateIndexOnNamesListUpdate(List<string> optionNames, SerializedProperty indexProperty, int oldSelectedPropertyIndex, string oldPropertyName, bool isOldPropertyNull)
        {
            if (isOldPropertyNull)
            {
                indexProperty.intValue = 0;
            }
            
            bool oldIndexIsOutOfBoundsOfNameListCount = oldSelectedPropertyIndex >= optionNames.Count;
            bool oldNameIsDifferentThanSelectedName = oldIndexIsOutOfBoundsOfNameListCount || oldPropertyName != optionNames[oldSelectedPropertyIndex];
            
            if (oldNameIsDifferentThanSelectedName)
            {
                if (optionNames.Contains(oldPropertyName))
                {
                    selectedDialogueGroupIndexProperty.intValue = optionNames.IndexOf(oldPropertyName);
                }
                else
                {
                    selectedDialogueGroupIndexProperty.intValue = 0;
                }
            }
        }
    }
}
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Enumerations;
    using Utilities;
    using Windows;

    public class DSNode : Node
    {
        public string DialogueName { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }
        public DSDialogueType DialogueType { get; set; }
        public DSGroup Group { get; set; }

        private DSGraphView graphView;

        private Color defaultBackgroundColor;

        public virtual void Initialize(DSGraphView dsGraphView, Vector2 position)
        {
            DialogueName = "NombreDelDiálogo";
            Choices = new List<string>();
            Text = "Texto del Diálogo.";

            graphView = dsGraphView;
            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        public virtual void Draw()
        {
            /* CONTENEDOR DEL TÍTULO */
            TextField dialogueNameTextField = DSElementUtility.CreateTextField(DialogueName, null, callback =>
            {
                TextField target = (TextField) callback.target;

                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
                
                
                if (Group == null)
                {
                    graphView.RemoveUngroupedNode(this);

                    DialogueName = target.value;

                    graphView.AddUngroupedNode(this);

                    return;
                }

                DSGroup currentGroup = Group;

                graphView.RemoveGroupedNode(this, Group);

                DialogueName = callback.newValue;

                graphView.AddGroupedNode(this, currentGroup);
            });

            dialogueNameTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__filename-textfield",
                "ds-node__textfield__hidden"
            );

            titleContainer.Insert(0, dialogueNameTextField);

            /* CONTENEDOR DE INPUT */
            Port inputPort = this.CreatePort("Conexión del Diálogo", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputContainer.Add(inputPort);

            /* CONTENEDOR DE EXTENSIONES */
            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            Foldout textFoldout = DSElementUtility.CreateFoldout("Texto del Diálogo");

            TextField textTextField = DSElementUtility.CreateTextArea(Text);

            textTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__quote-textfield"
            );

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);
        }
        
        #region Métodos Override
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Desconectar Puertos de Entrada", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Desconectar Puertos de Salida", actionEvent => DisconnectOutputPorts());
            
            if (!(evt.target is Node))
                return;
            evt.menu.AppendAction("Desconectar Todos los Puertos", actionEvent => DisconnectAllPorts());
            evt.menu.AppendSeparator();
        }
        #endregion

        #region Métodos de Utilidades
        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }
        
        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }
        
        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }
                
                graphView.DeleteElements(port.connections);
            }
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
        #endregion
    }
}

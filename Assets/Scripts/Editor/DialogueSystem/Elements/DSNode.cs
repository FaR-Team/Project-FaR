using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Enumerations;

    public class DSNode : Node
    {
        public string DialogueName { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }
        public DSDialogueType DialogueType { get; set; }

        public virtual void Initialize(Vector2 position)
        {
            DialogueName = "NombreDelDiálogo";
            Choices = new List<string>();
            Text = "Texto del Diálogo.";

            SetPosition(new Rect(position, Vector2.zero));
        }

        public virtual void Draw()
        {
            /* CONTENEDOR DEL TÍTULO */
            TextField dialogueNameTextField = new TextField()
            {
                value = DialogueName
            };

            titleContainer.Insert(0, dialogueNameTextField);

            /* CONTENEDOR DE INPUT */
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));

            inputPort.portName = "Conexión del Diálogo";

            inputContainer.Add(inputPort);

            /* CONTENEDOR DE EXTENSIONES */
            VisualElement customDataContainer = new VisualElement();

            Foldout textFoldout = new Foldout()
            {
                text = "Texto del Diálogo"
            };

            TextField textTextField = new TextField()
            {
                value = Text
            };

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);
        }
    }
}

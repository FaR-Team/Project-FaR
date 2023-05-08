using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace DS.Elements
{
    using Enumerations;

    public class DSMultipleChoiceNode : DSNode
    {
        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);

            DialogueType = DSDialogueType.MultipleChoice;

            Choices.Add("Nueva Opción");
        }

        public override void Draw()
        {
            base.Draw();

            /* CONTENEDOR PRINCIPAL */
            Button addChoiceButton = new Button()
            {
                text = "Añadir Opción"
            };

            mainContainer.Insert(1, addChoiceButton);

            /* CONTENEDOR DE OUTPUT */
            foreach (string choice in Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

                choicePort.portName = "";

                Button deleteChoiceButton = new Button()
                {
                    text = "X"
                };

                TextField choiceTextField = new TextField()
                {
                    value = choice
                };

                choicePort.Add(choiceTextField);
                choicePort.Add(deleteChoiceButton);


                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }
    }
}
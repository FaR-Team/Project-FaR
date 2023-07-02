using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DS.Windows
{  
    using Utilities;
    public class DSEditorWindow : EditorWindow
    {
        private readonly string defaultFileName = "NombreDeArchivo";
        private TextField fileNameTextField;
        private Button saveButton;
        
        [MenuItem("FARUtils/Dialogos/Dialogue Graph")]
        public static void Open()
        {
            GetWindow<DSEditorWindow>("Dialogue Graph");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolbar();

            AddStyles();
        }

        #region Adición de Elementos
        private void AddGraphView()
        {
            DSGraphView graphView = new DSGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            fileNameTextField = DSElementUtility.CreateTextField(defaultFileName, "Nombre del Archivo:", callback =>
                {
                    fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
                });

            saveButton = DSElementUtility.CreateButton("Guardar");
            
            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            
            toolbar.AddStyleSheets("DialogueSystem/DSToolbarStyles.uss");
            
            rootVisualElement.Add(toolbar);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("DialogueSystem/DSVariables.uss");
        }
        #endregion

        #region Utilidades
        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }
        
        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }
        #endregion
    }
}
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using FaRUtils.Assembly;

namespace FaRUtils.SOE
{
    public class CreationEditorWindow : EditorWindow
    {
        protected string scriptableName = "";

        protected string selectedPath = "Assets";
        protected System.Type selectedType = typeof(ScriptableObject);

        protected string typeName = "Nuevo tipo";
        protected Rect typeButton = new Rect();

        private string createdPath = "";

        private void OnEnable()
        {
            ShowPopup();
            position = new Rect(position.x, position.y, position.width, 150);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal("Box", GUILayout.ExpandWidth(true));
            if (GUILayout.Button(new GUIContent("Carpeta", "Se usa para seleccionar la carpeta en la que se va a crear el ScriptableObject"), GUILayout.MaxWidth(150)))
            {
                string basePath = EditorUtility.OpenFolderPanel("Seleccioná una carpeta.", selectedPath, "");

                if (basePath.Contains(Application.dataPath))
                {
                    basePath = basePath.Substring(basePath.LastIndexOf("Assets"));
                }

                if (AssetDatabase.IsValidFolder(basePath))
                {
                    selectedPath = basePath;
                }
            }

            EditorGUILayout.LabelField(selectedPath);
            EditorGUILayout.EndHorizontal();

            if (EditorGUILayout.DropdownButton(new GUIContent(typeName, "El tipo del que se va a basar el nuevo ScriptableObject."), FocusType.Keyboard))
            {
                GenericMenu menu = new GenericMenu();

                var function = new GenericMenu.MenuFunction2((type) => { selectedType = (System.Type)type; typeName = type.ToString(); if (selectedType == typeof(ScriptableObject)) typeName = "Nuevo Tipo"; });

                menu.AddItem(new GUIContent("Nuevo Tipo"), AssemblyTypes.OfType(typeof(ScriptableObject), selectedType), function, typeof(ScriptableObject));
                menu.AddSeparator("");

                foreach (var item in AssemblyTypes.GetAllTypes())
                {
                    menu.AddItem(new GUIContent(item.ToString()), AssemblyTypes.OfType(item, selectedType), function, item);
                }
                menu.DropDown(typeButton);
            }
            if (Event.current.type == EventType.Repaint) typeButton = GUILayoutUtility.GetLastRect();

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Nombre", "El nombre que se va a usar para el ScriptableObject o Tipo."), GUILayout.Width(40));
            scriptableName = EditorGUILayout.TextField(scriptableName);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(30);

            EditorGUILayout.BeginHorizontal("Box");
            if (GUILayout.Button(new GUIContent("Crear", "Create un nuevo ScriptableObject con la carpeta, tipo, y nombre seleccionados.")))
            {
                createdPath = Application.dataPath.Replace("Assets", "") + selectedPath + "/" + scriptableName + ".cs";

                switch (true)
                {
                    case bool _ when scriptableName.Length <= 0:
                        EditorUtility.DisplayDialog("Error: Se necesita nombre de archivo", "El nombre del archivo " + selectedType + " no puede quedar en blanco.", "Ok");
                        break;
                    case bool _ when !scriptableName.All(char.IsLetterOrDigit):
                        EditorUtility.DisplayDialog("Error: Se necesita nombre de archivo", "el nombre del archivo  " + selectedType + " no puede tener carácteres inválidos.", "Ok");
                        break;
                    case bool _ when File.Exists(createdPath):
                        EditorUtility.DisplayDialog("Error: El archivo ya existe.", "Un archivo con ese nombre ya existe en el directorio seleccionado", "Ok");
                        break;
                    case bool _ when selectedType == typeof(ScriptableObject):
                        var template = Resources.Load<TextAsset>("TemplateScriptable");
                        string contents = template.text;
                        contents = contents.Replace("DefaultName", scriptableName);

                        StreamWriter sw = new StreamWriter(createdPath);
                        sw.Write(contents);
                        sw.Close();
                        AssetDatabase.Refresh();
                        Created(true);
                        break;
                    default:
                        var type = selectedType;
                        Object newScriptable = AssemblyTypes.CreateObject(type);
                        AssetDatabase.CreateAsset(newScriptable, selectedPath + "/" + scriptableName + ".asset");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        Created(false);
                        break;
                }
            }

            if (GUILayout.Button(new GUIContent("Cerrar", "Salir del menú de creación."))) { Close(); }
            EditorGUILayout.EndHorizontal();
        }

        public void Created(bool newType)
        {
            switch (newType)
            {
                case true:
                    switch (EditorUtility.DisplayDialog(typeName + " Creado! ", typeName + " '" + scriptableName + "' se creó correctamente! ¿Querés abrir y modificar sus contenidos?", "Sí", "No"))
                    {
                        case true:
                            Application.OpenURL(createdPath);
                            Close();
                            break;
                        case false:
                            Close();
                            break;
                    }
                    break;
                case false:
                    switch (EditorUtility.DisplayDialog(typeName + " Creado! ", typeName + " '" + scriptableName + "' se creó correctamente!", "Ok"))
                    {
                        case true:
                            Close();
                            break;
                    }
                    break;
            }

        }
    }
}
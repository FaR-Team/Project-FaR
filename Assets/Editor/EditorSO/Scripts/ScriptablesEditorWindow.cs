using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using FaRUtils.Assembly;

namespace FaRUtils.SOE
{
    public class ScriptablesEditorWindow : EditorWindow
    {
        public static ScriptablesEditorWindow window;

        protected GUISkin skin;

        protected SerializedObject serializedObject;
        protected SerializedProperty serializedProperty;

        protected Object selectedObject;
        protected string selectedName;
        protected ScriptableObject[] activeObjects;
        protected string selectedPropertyPach;
        protected string selectedProperty;

        Vector2 scrollPosition = Vector2.zero;
        Vector2 itemScrollPosition = Vector2.zero;
        readonly float sidebarWidth = 250f;

        protected string activePath = "Assets";
        protected System.Type activeType = typeof(ScriptableObject);

        protected string typeName = "Tipo de SO";

        protected string sortSearch = "";
        protected int stringMax = 27;

        protected Rect typeButton;

        [MenuItem("FARUtils/Editor de Scriptable Objects")]
        protected static void ShowWindow()
        {
            window = GetWindow<ScriptablesEditorWindow>("Editor de Scriptable Objects");
            window.UpdateObjets();
        }

        private void OnEnable()
        {
            skin = (GUISkin)Resources.Load("ScriptableEditorGUI");
            UpdateObjets();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");

            HeaderTitle();
            HeaderNavigation();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();

            SelectionNavigation();
            SelectableContents();

            EditorGUILayout.EndHorizontal();

            if (activeObjects.Length > 0 && serializedObject != null)
                Apply();
        }

        private void HeaderTitle()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Editor de Scriptable Objects", skin.customStyles.ToList().Find(x => x.name == "Header"));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        #region Navegación
        #region Encabezado
        private void HeaderNavigation()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            if (GUILayout.Button(new GUIContent("Carpeta", "Permite cambiar el directorio de búsqueda."), GUILayout.MaxWidth(150)))
            {
                string basePath = EditorUtility.OpenFolderPanel("Seleccioná la carpeta.", activePath, "");

                if (basePath.Contains(Application.dataPath))
                {
                    basePath = basePath.Substring(basePath.LastIndexOf("Assets"));
                }

                if (!AssetDatabase.IsValidFolder(basePath))
                {
                    EditorUtility.DisplayDialog("Error: Directorio inválido", "Asegurate que la carpeta esté en el directorio del proyecto, adentro de Assets", "Ok");
                }
                else
                {
                    activePath = basePath;
                    UpdateObjets();
                }

            }

            EditorGUILayout.LabelField(activePath);
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Sidebar
        private void SelectionNavigation()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(sidebarWidth), GUILayout.ExpandHeight(true));

            if (EditorGUILayout.DropdownButton(new GUIContent(typeName, "Se usa para filtrar el tipo de SO."), FocusType.Keyboard))
            {
                GenericMenu menu = new GenericMenu();

                var function = new GenericMenu.MenuFunction2((type) => { activeType = (System.Type)type; typeName = type.ToString(); if (activeType == typeof(ScriptableObject)) typeName = "Todos"; UpdateObjets(); });

                menu.AddItem(new GUIContent("Todos", "Mostrar todos los tipos de ScriptableObject en el proyecto."), AssemblyTypes.OfType(typeof(ScriptableObject), activeType), function, typeof(ScriptableObject));
                menu.AddSeparator("");

                foreach (var item in AssemblyTypes.GetAllTypes())
                {
                    menu.AddItem(new GUIContent(item.ToString()), AssemblyTypes.OfType(item, activeType), function, item);
                }
                menu.DropDown(typeButton);
            }
            if (Event.current.type == EventType.Repaint) typeButton = GUILayoutUtility.GetLastRect();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            sortSearch = EditorGUILayout.TextField(sortSearch, GUILayout.MaxWidth(240));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandHeight(true));

            if (activeObjects.Length > 0)
                DrawScriptables(activeObjects);

            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("+", "Abre el menú de creación de ScriptableObjects"), GUILayout.Width(25)))
            {
                CreationEditorWindow window = GetWindow<CreationEditorWindow>();
                window.position = AssemblyTypes.CenterOnOriginWindow(window.position, position);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        #endregion
        #endregion

        #region Mostrar Contenidos Seleccionados
        private void SelectableContents()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
            itemScrollPosition = EditorGUILayout.BeginScrollView(itemScrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandHeight(true));
            switch (true)
            {
                case bool _ when serializedObject != null && selectedObject != null:
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Name"));
                    GUI.enabled = true;

                    serializedProperty = serializedObject.GetIterator();
                    serializedProperty.NextVisible(true);
                    DrawProperties(serializedProperty);

                    GUILayout.Space(15);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent("Renombrar", "Se usa para renombrar el ScriptableObject seleccionado.")))
                    {
                        RenamePopup(selectedObject);
                    }

                    var style = new GUIStyle(GUI.skin.button);
                    style.normal.textColor = Color.red;

                    if (GUILayout.Button(new GUIContent("Eliminar", "Elimina el ScriptableObject seleccionado."), style))
                    {
                        switch (EditorUtility.DisplayDialog("¿Eliminar " + selectedObject.name + "?", "¿Seguro que querés eliminar '" + selectedObject.name + "'?", "Sí", "No"))
                        {
                            case true:
                                string path = AssetDatabase.GetAssetPath(selectedObject);
                                AssetDatabase.DeleteAsset(path);
                                serializedObject = null;
                                selectedObject = null;
                                UpdateObjets();
                                break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    break;
                default:
                    EditorGUILayout.LabelField("No hay nada seleccionado, asegurate que seleccionaste algo.");
                    break;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        protected void RenamePopup(Object selected)
        {
            PopupWindow.Show(new Rect(), new NamingEditorWindow(selected, position));
        }
        #endregion

        protected void UpdateObjets()
        {
            activeObjects = AssemblyTypes.GetAllInstancesOfType(activePath, activeType);
        }

        protected void DrawProperties(SerializedProperty property)
        {
            if (property.displayName == "Script") { GUI.enabled = false; }
            EditorGUILayout.PropertyField(property, true);
            GUI.enabled = true;

            EditorGUILayout.Space(40);

            while (property.NextVisible(false))
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }

        protected void DrawScriptables(ScriptableObject[] objects)
        {
            foreach (ScriptableObject item in objects)
            {
                if (item != null && item.name.IndexOf(sortSearch, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (GUILayout.Button(ShortenString(item.name), skin.button, GUILayout.ExpandWidth(true)))
                    {
                        if (Event.current.button == 1)
                        {
                            GenericMenu menu = new GenericMenu();

                            var function = new GenericMenu.MenuFunction2((name) => RenamePopup(AssemblyTypes.FindObject(activeObjects, item.name)));

                            menu.AddItem(new GUIContent("Renombrar"), false, function, item.name);
                            menu.ShowAsContext();
                        }
                        else
                        {
                            selectedPropertyPach = item.name;

                            if (!string.IsNullOrEmpty(selectedPropertyPach))
                            {
                                selectedProperty = selectedPropertyPach;
                                selectedObject = AssemblyTypes.FindObject(activeObjects, selectedProperty);
                            }

                            UpdateObjets();
                        }
                    }
                }
            }

            switch (true)
            {
                case bool _ when selectedObject != null:
                    serializedObject = new SerializedObject(selectedObject);
                    break;
            }
        }

        protected string ShortenString(string item)
        {
            switch (true)
            {
                case bool _ when item.Length >= stringMax:
                    return item.Substring(0, stringMax) + "...";
                default:
                    return item;
            }
        }

        protected void Apply()
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
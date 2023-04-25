using UnityEditor;
using UnityEngine;
using FaRUtils.Assembly;

namespace FaRUtils.SOE
{
    public class NamingEditorWindow : PopupWindowContent
    {
        protected string newName = "Escribí un nuevo nombre";
        protected Object selectedObject;
        protected Rect position;

        protected Vector2 dimensions = new Vector2(300, 125);

        public NamingEditorWindow(Object selected, Rect origin)
        {
            selectedObject = selected;
            position = origin;
        }

        public override void OnGUI(Rect rect)
        {
            editorWindow.position = AssemblyTypes.CenterOnOriginWindow(editorWindow.position, position);

            EditorGUILayout.Space(10);
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = 12;
            style.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("Renombrar " + selectedObject.name, style);
            EditorGUILayout.Space(10);

            newName = EditorGUILayout.TextField(newName);

            if (GUILayout.Button("Renombrar"))
            {
                var path = AssetDatabase.GetAssetPath(selectedObject);
                AssetDatabase.RenameAsset(path, newName);
                editorWindow.Close();
            }

            if (GUILayout.Button("Cancelar"))
            {
                editorWindow.Close();
            }

            GUILayout.FlexibleSpace();
        }

        public override Vector2 GetWindowSize()
        {
            return dimensions;
        }
    }
}
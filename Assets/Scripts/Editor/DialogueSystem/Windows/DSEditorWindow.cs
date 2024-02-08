using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DS.Windows
{
    
    public class DSEditorWindow : EditorWindow
    {
        [MenuItem("FARUtils/Dialogos/Dialogue Graph")]
        public static void ShowExample()
        {
            GetWindow<DSEditorWindow>("Dialogue Graph");
        }
    }
}
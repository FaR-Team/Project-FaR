namespace FARUtils.Notes
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class FaRNoteStyleWindow : EditorWindow
    {
        private FaRNoteStyleConfig config;
        private VisualElement root;
        private StyleSheet styleSheet;

        [MenuItem("FARUtils/FaR Notes/Style Settings")]
        public static void Open()
        {
            GetWindow<FaRNoteStyleWindow>("Scene Notes Style");
        }

        private void OnEnable()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            string[] guids = AssetDatabase.FindAssets("t:FaRNoteStyleConfig");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                config = AssetDatabase.LoadAssetAtPath<FaRNoteStyleConfig>(path);
            }
        }

        private void LoadStyleSheet()
        {
            string[] guids = AssetDatabase.FindAssets("FaRNotesStyles t:StyleSheet");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
                if (styleSheet != null)
                {
                    root.styleSheets.Add(styleSheet);
                }
            }
        }

        private void CreateGUI()
        {
            root = rootVisualElement;
            root.AddToClassList("far-inspector-root");
            LoadStyleSheet();

            if (config == null)
            {
                root.Add(new HelpBox("No Style Config found.", HelpBoxMessageType.Warning));
                var createBtn = new Button(() => {
                    config = CreateInstance<FaRNoteStyleConfig>();
                    AssetDatabase.CreateAsset(config, "Assets/Resources/FaR Notes/FaRNoteStyleConfig.asset");
                    AssetDatabase.SaveAssets();
                    CreateGUI();
                }) { text = "Create Style Config" };
                createBtn.AddToClassList("far-button");
                root.Add(createBtn);
                return;
            }

            var title = new Label("Style Settings");
            title.AddToClassList("far-title");
            root.Add(title);

            var scrollView = new ScrollView();
            root.Add(scrollView);

            var so = new SerializedObject(config);
            var prop = so.GetIterator();
            prop.NextVisible(true);

            while (prop.NextVisible(false))
            {
                var field = new PropertyField(prop);
                field.Bind(so);
                field.RegisterValueChangeCallback(evt => {
                    SceneView.RepaintAll();
                    OnStyleChanged?.Invoke();
                });
                scrollView.Add(field);
            }
        }

        public static event System.Action OnStyleChanged;
    }
#endif
}

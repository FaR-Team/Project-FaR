namespace FARUtils.Notes
{


#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Overlays;
    using UnityEngine;
    using UnityEngine.UIElements;

    [Overlay(typeof(SceneView), "FaR Notes")]
    public class FaRNoteOverlay : Overlay
    {
        public static FaRNoteFilterState filterState = new FaRNoteFilterState();

        private VisualElement root;
        private StyleSheet styleSheet;

        public override VisualElement CreatePanelContent()
        {
            filterState.EnsureInitialized();

            root = new VisualElement();
            root.AddToClassList("far-overlay-root");

            LoadStyleSheet();
            BuildUI();

            return root;
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

        private void BuildUI()
        {
            root.Clear();

            // Header/Title
            var header = new Label("SCENE NOTES");
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.fontSize = 10;
            header.style.color = new Color(0.6f, 0.6f, 0.6f);
            header.style.marginBottom = 4;
            header.style.unityTextAlign = TextAnchor.MiddleCenter;
            root.Add(header);

            // Create Button
            var createButton = new Button(() => FaRNoteTool.SetCreateMode(true))
            {
                text = "Create Note"
            };
            createButton.AddToClassList("far-button");
            root.Add(createButton);

            root.Add(new VisualElement { style = { height = 8 } });

            // Master Toggle
            var masterToggle = new Toggle("Activate All")
            {
                value = filterState.AreAllActive()
            };
            masterToggle.AddToClassList("far-toggle");

            masterToggle.RegisterValueChangedCallback(evt =>
            {
                filterState.SetAll(evt.newValue);
                SceneView.RepaintAll();
                RefreshToggles();
            });

            root.Add(masterToggle);
            
            var separator = new VisualElement { style = { height = 1, backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.5f), marginTop = 6, marginBottom = 6 } };
            root.Add(separator);

            // Category Toggles Container
            var categoriesLabel = new Label("CATEGORIES");
            categoriesLabel.style.fontSize = 9;
            categoriesLabel.style.color = new Color(0.5f, 0.5f, 0.5f);
            root.Add(categoriesLabel);

            foreach (FaRNoteCategory category in System.Enum.GetValues(typeof(FaRNoteCategory)))
            {
                var toggle = new Toggle(category.ToString())
                {
                    value = filterState.IsVisible(category)
                };
                toggle.AddToClassList("far-toggle");

                toggle.RegisterValueChangedCallback(evt =>
                {
                    filterState.Toggle(category, evt.newValue);
                    SceneView.RepaintAll();
                    // We can't easily find the masterToggle here without keeping a reference or searching
                    // but we can just update its value if we store it.
                    UpdateMasterToggle(masterToggle);
                });

                root.Add(toggle);
            }
        }

        private void UpdateMasterToggle(Toggle masterToggle)
        {
            masterToggle.SetValueWithoutNotify(filterState.AreAllActive());
        }

        private void RefreshToggles()
        {
            foreach (var element in root.Children())
            {
                if (element is Toggle toggle && !toggle.label.Equals("Activate All"))
                {
                    if (System.Enum.TryParse(toggle.label, out FaRNoteCategory category))
                        toggle.SetValueWithoutNotify(filterState.IsVisible(category));
                }
            }
        }
    }
#endif
}
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using static FaRFolders.Libs.VUtils;
using static FaRFolders.Libs.VGUI;

namespace FaRFolders
{
    public class FaRFoldersMenu : EditorWindow
    {
        public static bool clearerRowsEnabled { get => EditorPrefs.GetBool("FaRFolders-clearerRowsEnabled", true); set => EditorPrefs.SetBool("FaRFolders-clearerRowsEnabled", value); }
        public static bool minimalModeEnabled { get => EditorPrefs.GetBool("FaRFolders-minimalModeEnabled", false); set => EditorPrefs.SetBool("FaRFolders-minimalModeEnabled", value); }
        public static bool contentMinimapEnabled { get => EditorPrefs.GetBool("FaRFolders-contentMinimapEnabled", true); set => EditorPrefs.SetBool("FaRFolders-contentMinimapEnabled", value); }
        public static bool hierarchyLinesEnabled { get => EditorPrefs.GetBool("FaRFolders-hierarchyLinesEnabled", true); set => EditorPrefs.SetBool("FaRFolders-hierarchyLinesEnabled", value); }
        public static bool zebraStripingEnabled { get => EditorPrefs.GetBool("FaRFolders-zebraStripingEnabled", true); set => EditorPrefs.SetBool("FaRFolders-zebraStripingEnabled", value); }
        public static bool autoIconsEnabled { get => EditorPrefs.GetBool("FaRFolders-autoIconsEnabled", true); set => EditorPrefs.SetBool("FaRFolders-autoIconsEnabled", value); }
        public static bool foldersFirstEnabled { get => EditorPrefs.GetBool("FaRFolders-foldersFirstEnabled", false); set => EditorPrefs.SetBool("FaRFolders-foldersFirstEnabled", value); }
        public static bool toggleExpandedEnabled { get => EditorPrefs.GetBool("FaRFolders-toggleExpandedEnabled", true); set => EditorPrefs.SetBool("FaRFolders-toggleExpandedEnabled", value); }
        public static bool collapseEverythingElseEnabled { get => EditorPrefs.GetBool("FaRFolders-collapseEverythingElseEnabled", true); set => EditorPrefs.SetBool("FaRFolders-collapseEverythingElseEnabled", value); }
        public static bool collapseEverythingEnabled { get => EditorPrefs.GetBool("FaRFolders-collapseEverythingEnabled", true); set => EditorPrefs.SetBool("FaRFolders-collapseEverythingEnabled", value); }
        public static bool pluginDisabled { get => EditorPrefs.GetBool("FaRFolders-pluginDisabled", false); set => EditorPrefs.SetBool("FaRFolders-pluginDisabled", value); }

        [MenuItem("FARUtils/FaR Folders/Settings", false, 1)]
        public static void Open()
        {
            GetWindow<FaRFoldersMenu>("FaR Folders Settings");
        }

        [MenuItem("FARUtils/FaR Folders/Disable FaR Folders", false, 100)]
        static void TogglePlugin()
        {
            pluginDisabled = !pluginDisabled;
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }

        [MenuItem("FARUtils/FaR Folders/Disable FaR Folders", true)]
        static bool TogglePluginValidate()
        {
            Menu.SetChecked("FARUtils/FaR Folders/Disable FaR Folders", pluginDisabled);
            return true;
        }

        private void OnEnable()
        {
            minSize = new Vector2(350, 450);
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            root.AddToClassList("far-window-root");

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/FARUtils/UI/FaRSharedStyles.uss");
            if (styleSheet != null)
                root.styleSheets.Add(styleSheet);

            // Header
            var header = new VisualElement();
            header.AddToClassList("far-header");
            root.Add(header);

            var title = new Label("FaR Folders");
            title.AddToClassList("far-title");
            header.Add(title);

            var subtitle = new Label("Enhance your project directory with style and clarity.");
            subtitle.AddToClassList("far-subtitle");
            header.Add(subtitle);

            var scrollView = new ScrollView();
            root.Add(scrollView);

            // Features Section
            var featuresSection = CreateSection(scrollView, "Visual Features");
            CreateToggle(featuresSection, "Hierarchy Lines", hierarchyLinesEnabled, (v) => { hierarchyLinesEnabled = v; RepaintProject(); });
            CreateToggle(featuresSection, "Clearer Rows", clearerRowsEnabled, (v) => { clearerRowsEnabled = v; RepaintProject(); });
            CreateToggle(featuresSection, "Minimal Mode", minimalModeEnabled, (v) => { minimalModeEnabled = v; RepaintProject(); });
            CreateToggle(featuresSection, "Zebra Striping", zebraStripingEnabled, (v) => { zebraStripingEnabled = v; RepaintProject(); });
            CreateToggle(featuresSection, "Content Minimap", contentMinimapEnabled, (v) => { contentMinimapEnabled = v; RepaintProject(); });
            CreateToggle(featuresSection, "Automatic Icons", autoIconsEnabled, (v) => { autoIconsEnabled = v; RepaintProject(); });

            // Behavior Section
            var behaviorSection = CreateSection(scrollView, "Behavior");
            CreateToggle(behaviorSection, "Sort Folders First", foldersFirstEnabled, (v) => { foldersFirstEnabled = v; RepaintProject(); if (!v) UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation(); });

            // Shortcuts Section
            var shortcutsSection = CreateSection(scrollView, "Shortcuts");
            CreateToggle(shortcutsSection, "E: Expand / Collapse", toggleExpandedEnabled, (v) => toggleExpandedEnabled = v);
            CreateToggle(shortcutsSection, "Shift-E: Collapse Others", collapseEverythingElseEnabled, (v) => collapseEverythingElseEnabled = v);
            CreateToggle(shortcutsSection, "Ctrl-Shift-E: Collapse All", collapseEverythingEnabled, (v) => collapseEverythingEnabled = v);

            // Status Section
            var statusSection = CreateSection(scrollView, "Status");
            var pluginStatus = new Label(pluginDisabled ? "Plugin is currently DISABLED" : "Plugin is ACTIVE");
            pluginStatus.style.color = pluginDisabled ? Color.red : Color.green;
            pluginStatus.style.fontSize = 11;
            statusSection.Add(pluginStatus);

            var toggleBtn = new Button(() => { pluginDisabled = !pluginDisabled; UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation(); Close(); }) { text = pluginDisabled ? "Enable Plugin" : "Disable Plugin" };
            toggleBtn.AddToClassList("far-button");
            statusSection.Add(toggleBtn);

            // Footer
            var footer = new VisualElement();
            footer.AddToClassList("far-footer");
            root.Add(footer);

            var version = new Label("v2.1.0-FaR");
            version.AddToClassList("far-version-label");
            footer.Add(version);
        }

        private VisualElement CreateSection(VisualElement parent, string title)
        {
            var section = new VisualElement();
            section.AddToClassList("far-section");
            parent.Add(section);

            var header = new Label(title);
            header.AddToClassList("far-section-title");
            section.Add(header);

            return section;
        }

        private void CreateToggle(VisualElement section, string label, bool value, System.Action<bool> onValueChanged)
        {
            var container = new VisualElement();
            container.AddToClassList("far-field-container");
            section.Add(container);

            var nameLabel = new Label(label);
            nameLabel.AddToClassList("far-field-label");
            container.Add(nameLabel);

            var toggle = new Toggle();
            toggle.value = value;
            toggle.AddToClassList("far-toggle");
            toggle.RegisterValueChangedCallback(evt => onValueChanged?.Invoke(evt.newValue));
            container.Add(toggle);
        }

        private void RepaintProject() => EditorApplication.RepaintProjectWindow();
    }
}
#endif
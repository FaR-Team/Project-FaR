#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using static FaRHierarchy.Libs.VUtils;
using static FaRHierarchy.Libs.VGUI;

namespace FaRHierarchy
{
    public class FaRHierarchyMenu : EditorWindow
    {
        public static bool navigationBarEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-navigationBarEnabled", false); set => EditorPrefsCached.SetBool("FaRHierarchy-navigationBarEnabled", value); }
        public static bool sceneSelectorEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-sceneSelectorEnabled", false); set => EditorPrefsCached.SetBool("FaRHierarchy-sceneSelectorEnabled", value); }
        public static bool componentMinimapEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-componentMinimapEnabled", false); set => EditorPrefsCached.SetBool("FaRHierarchy-componentMinimapEnabled", value); }
        public static bool activationToggleEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-acctivationToggleEnabled", false); set => EditorPrefsCached.SetBool("FaRHierarchy-acctivationToggleEnabled", value); }
        public static bool hierarchyLinesEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-hierarchyLinesEnabled", false); set => EditorPrefsCached.SetBool("FaRHierarchy-hierarchyLinesEnabled", value); }
        public static bool minimalModeEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-minimalModeEnabled", false); set => EditorPrefsCached.SetBool("FaRHierarchy-minimalModeEnabled", value); }
        public static bool zebraStripingEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-zebraStripingEnabled", false); set => EditorPrefsCached.SetBool("FaRHierarchy-zebraStripingEnabled", value); }

        public static bool toggleActiveEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-toggleActiveEnabled", true); set => EditorPrefsCached.SetBool("FaRHierarchy-toggleActiveEnabled", value); }
        public static bool focusEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-focusEnabled", true); set => EditorPrefsCached.SetBool("FaRHierarchy-focusEnabled", value); }
        public static bool deleteEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-deleteEnabled", true); set => EditorPrefsCached.SetBool("FaRHierarchy-deleteEnabled", value); }
        public static bool toggleExpandedEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-toggleExpandedEnabled", true); set => EditorPrefsCached.SetBool("FaRHierarchy-toggleExpandedEnabled", value); }
        public static bool isolateEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-collapseEverythingElseEnabled", true); set => EditorPrefsCached.SetBool("FaRHierarchy-collapseEverythingElseEnabled", value); }
        public static bool collapseEverythingEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-collapseEverythingEnabled", true); set => EditorPrefsCached.SetBool("FaRHierarchy-collapseEverythingEnabled", value); }
        public static bool setDefaultParentEnabled { get => EditorPrefsCached.GetBool("FaRHierarchy-setDefaultParentEnabled", true); set => EditorPrefsCached.SetBool("FaRHierarchy-setDefaultParentEnabled", value); }

        public static bool pluginDisabled { get => EditorPrefsCached.GetBool("FaRHierarchy-pluginDisabled", false); set => EditorPrefsCached.SetBool("FaRHierarchy-pluginDisabled", value); }

        [MenuItem("FARUtils/FaR Hierarchy/Settings", false, 1)]
        public static void Open()
        {
            GetWindow<FaRHierarchyMenu>("FaR Hierarchy Settings");
        }

        [MenuItem("FARUtils/FaR Hierarchy/Disable FaR Hierarchy", false, 100)]
        static void TogglePlugin()
        {
            pluginDisabled = !pluginDisabled;
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }

        [MenuItem("FARUtils/FaR Hierarchy/Disable FaR Hierarchy", true)]
        static bool TogglePluginValidate()
        {
            Menu.SetChecked("FARUtils/FaR Hierarchy/Disable FaR Hierarchy", pluginDisabled);
            return true;
        }

        private void OnEnable()
        {
            minSize = new Vector2(350, 550);
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

            var title = new Label("FaR Hierarchy");
            title.AddToClassList("far-title");
            header.Add(title);

            var subtitle = new Label("A cleaner, faster, and more powerful hierarchy for Unity.");
            subtitle.AddToClassList("far-subtitle");
            header.Add(subtitle);

            var scrollView = new ScrollView();
            root.Add(scrollView);

            // Appearance Section
            var appearanceSection = CreateSection(scrollView, "Appearance");
            CreateToggle(appearanceSection, "Hierarchy Lines", hierarchyLinesEnabled, (v) => { hierarchyLinesEnabled = v; RepaintHierarchy(); });
            CreateToggle(appearanceSection, "Zebra Striping", zebraStripingEnabled, (v) => { zebraStripingEnabled = v; RepaintHierarchy(); });
            CreateToggle(appearanceSection, "Minimal Mode", minimalModeEnabled, (v) => { minimalModeEnabled = v; RepaintHierarchy(); });
            CreateToggle(appearanceSection, "Navigation Bar", navigationBarEnabled, (v) => { navigationBarEnabled = v; RepaintHierarchy(); });
            CreateToggle(appearanceSection, "Scene Selector", sceneSelectorEnabled, (v) => { sceneSelectorEnabled = v; RepaintHierarchy(); });
            CreateToggle(appearanceSection, "Component Minimap", componentMinimapEnabled, (v) => { componentMinimapEnabled = v; RepaintHierarchy(); });
            CreateToggle(appearanceSection, "Activation Toggle", activationToggleEnabled, (v) => { activationToggleEnabled = v; RepaintHierarchy(); });

            // Shortcuts Section
            var shortcutsSection = CreateSection(scrollView, "Shortcuts");
            CreateToggle(shortcutsSection, "A: Toggle Active", toggleActiveEnabled, (v) => toggleActiveEnabled = v);
            CreateToggle(shortcutsSection, "F: Focus In Scene", focusEnabled, (v) => focusEnabled = v);
            CreateToggle(shortcutsSection, "X: Delete GameObject", deleteEnabled, (v) => deleteEnabled = v);
            CreateToggle(shortcutsSection, "E: Expand / Collapse", toggleExpandedEnabled, (v) => toggleExpandedEnabled = v);
            CreateToggle(shortcutsSection, "Shift-E: Isolate", isolateEnabled, (v) => isolateEnabled = v);
            CreateToggle(shortcutsSection, "Ctrl-Shift-E: Collapse All", collapseEverythingEnabled, (v) => collapseEverythingEnabled = v);
            CreateToggle(shortcutsSection, "D: Set Default Parent", setDefaultParentEnabled, (v) => setDefaultParentEnabled = v);

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

            var version = new Label("v2.5.2-FaR");
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

        private void RepaintHierarchy() => EditorApplication.RepaintHierarchyWindow();
    }
}
#endif
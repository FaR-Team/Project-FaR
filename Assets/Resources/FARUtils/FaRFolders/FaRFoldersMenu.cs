#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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




        const string dir = "FARUtils/FaR Folders/";

        const string hierarchyLines = dir + "Hierarchy lines";
        const string clearerRows = dir + "Clearer rows";
        const string minimalMode = dir + "Minimal mode";
        const string zebraStriping = dir + "Zebra striping";
        const string autoIcons = dir + "Automatic icons";
        const string contentMinimap = dir + "Content minimap";
        const string foldersFirst = dir + "Sort folders first";

        const string toggleExpanded = dir + "E to expand or collapse";
        const string collapseEverythingElse = dir + "Shift-E to collapse everything else";
        const string collapseEverything = dir + "Ctrl-Shift-E to collapse everything";

        const string disablePlugin = dir + "Disable FaRFolders";






        [MenuItem(dir + "Features", false, 101)] static void daasddsas() { }
        [MenuItem(dir + "Features", true, 101)] static bool dadsdasas123() => false;

        [MenuItem(hierarchyLines, false, 102)] static void dadsadadsadass() { hierarchyLinesEnabled = !hierarchyLinesEnabled; EditorApplication.RepaintProjectWindow(); }
        [MenuItem(hierarchyLines, true, 102)] static bool dadsaddasaasddsas() { Menu.SetChecked(hierarchyLines, hierarchyLinesEnabled); return !pluginDisabled; }

        [MenuItem(clearerRows, false, 103)] static void dadsadadsadsadass() { clearerRowsEnabled = !clearerRowsEnabled; EditorApplication.RepaintProjectWindow(); }
        [MenuItem(clearerRows, true, 103)] static bool dadsaddasadsaasddsas() { Menu.SetChecked(clearerRows, clearerRowsEnabled); return !pluginDisabled; }

        [MenuItem(minimalMode, false, 104)] static void dadsadadsaddsasadass() { minimalModeEnabled = !minimalModeEnabled; EditorApplication.RepaintProjectWindow(); }
        [MenuItem(minimalMode, true, 104)] static bool dadsaddasadsadsaasddsas() { Menu.SetChecked(minimalMode, minimalModeEnabled); return !pluginDisabled; }

        [MenuItem(zebraStriping, false, 105)] static void dadsadaddsasadsadass() { zebraStripingEnabled = !zebraStripingEnabled; EditorApplication.RepaintProjectWindow(); }
        [MenuItem(zebraStriping, true, 105)] static bool dadsaddadassadsaasddsas() { Menu.SetChecked(zebraStriping, zebraStripingEnabled); return !pluginDisabled; }

        [MenuItem(contentMinimap, false, 106)] static void dadsadadasdsadass() { contentMinimapEnabled = !contentMinimapEnabled; EditorApplication.RepaintProjectWindow(); }
        [MenuItem(contentMinimap, true, 106)] static bool dadsadddasasaasddsas() { Menu.SetChecked(contentMinimap, contentMinimapEnabled); return !pluginDisabled; }

        [MenuItem(autoIcons, false, 107)] static void dadsadadsas() { autoIconsEnabled = !autoIconsEnabled; EditorApplication.RepaintProjectWindow(); }
        [MenuItem(autoIcons, true, 107)] static bool dadsaddasadsas() { Menu.SetChecked(autoIcons, autoIconsEnabled); return !pluginDisabled; }
#if UNITY_EDITOR_OSX
        [MenuItem(foldersFirst, false, 108)] static void dadsdsfaadsdadsas() { foldersFirstEnabled = !foldersFirstEnabled; EditorApplication.RepaintProjectWindow(); if (!foldersFirstEnabled) UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation(); }
        [MenuItem(foldersFirst, true, 108)] static bool dadsasdfdadsdasadsas() { Menu.SetChecked(foldersFirst, foldersFirstEnabled); return !pluginDisabled; }
#endif



        [MenuItem(dir + "Shortcuts", false, 109)] static void dadsas() { }
        [MenuItem(dir + "Shortcuts", true, 109)] static bool dadsas123() => false;

        [MenuItem(toggleExpanded, false, 110)] static void dadsadsadasdsadadsas() => toggleExpandedEnabled = !toggleExpandedEnabled;
        [MenuItem(toggleExpanded, true, 110)] static bool dadsaddsasadadsdasadsas() { Menu.SetChecked(toggleExpanded, toggleExpandedEnabled); return !pluginDisabled; }

        [MenuItem(collapseEverythingElse, false, 111)] static void dadsadsasdadasdsadadsas() => collapseEverythingElseEnabled = !collapseEverythingElseEnabled;
        [MenuItem(collapseEverythingElse, true, 111)] static bool dadsaddsdasasadadsdasadsas() { Menu.SetChecked(collapseEverythingElse, collapseEverythingElseEnabled); return !pluginDisabled; }

        [MenuItem(collapseEverything, false, 112)] static void dadsadsdasadasdsadadsas() => collapseEverythingEnabled = !collapseEverythingEnabled;
        [MenuItem(collapseEverything, true, 112)] static bool dadsaddssdaasadadsdasadsas() { Menu.SetChecked(collapseEverything, collapseEverythingEnabled); return !pluginDisabled; }





        [MenuItem(disablePlugin, false, 113)] static void dadsadsdasadasdasdsadadsas() { pluginDisabled = !pluginDisabled; UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation(); }
        [MenuItem(disablePlugin, true, 113)] static bool dadsaddssdaasadsadadsdasadsas() { Menu.SetChecked(disablePlugin, pluginDisabled); return true; }




        // [MenuItem(dir + "Clear cache", false, 10001)]
        // static void dassaadsdc() => FaRFoldersCache.Clear();

    }
}
#endif
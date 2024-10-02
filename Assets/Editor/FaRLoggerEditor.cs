using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using Utils;

[InitializeOnLoad]
public class FaRConsoleWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private List<LogEntry> logEntries = new List<LogEntry>();
    private bool isCollapsed = false;
    private string[] clearOptions = new string[] { "Clear", "Clear on Play", "Clear on Build" };
    private int selectedClearOption = 0;

    [MenuItem("FARUtils/FaR Console")]
    public static void ShowWindow()
    {
        GetWindow<FaRConsoleWindow>("FaR Console");
    }

    static FaRConsoleWindow()
    {
        EditorApplication.delayCall += OpenWindow;
    }

    private static void OpenWindow()
    {
        GetWindow<FaRConsoleWindow>("FaR Console");
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        Application.logMessageReceivedThreaded += HandleLogThreaded;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
        Application.logMessageReceivedThreaded -= HandleLogThreaded;
    }

    private void HandleLogThreaded(string condition, string stackTrace, LogType type)
    {
        if (this.errorPause && (type == LogType.Error || type == LogType.Exception))
        {
            EditorApplication.isPaused = true;
        }
    }
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        var (prefix, objectName, message, fileName, lineNumber) = ParseLogString(logString);
        logEntries.Add(new LogEntry(prefix, objectName, message, stackTrace, type, fileName, lineNumber));
        Repaint();
    }
    private (string prefix, string objectName, string message, string fileName, int lineNumber) ParseLogString(string logString)
    {
        string prefix = "";
        string objectName = "";
        string message = logString;
        string fileName = "";
        int lineNumber = -1;

        int prefixEnd = logString.IndexOf(']');
        if (prefixEnd != -1)
        {
            prefix = logString.Substring(0, prefixEnd + 1);
            message = logString.Substring(prefixEnd + 1).Trim();

            int objectNameStart = prefix.IndexOf('[');
            int objectNameEnd = prefix.IndexOf(']');
            if (objectNameStart != -1 && objectNameEnd != -1)
            {
                objectName = prefix.Substring(objectNameStart + 1, objectNameEnd - objectNameStart - 1);
                prefix = prefix.Substring(0, objectNameStart);
            }
        }

        string[] lines = message.Split('\n');
        foreach (var line in lines)
        {
            if (line.StartsWith("File: "))
                fileName = line.Substring(6).Trim();
            else if (line.StartsWith("Line: "))
                int.TryParse(line.Substring(6).Trim(), out lineNumber);
        }

        return (prefix, objectName, message, fileName, lineNumber);
    }
    
    private bool errorPause = false;
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        // Clear dropdown
        if (EditorGUILayout.DropdownButton(new GUIContent(clearOptions[selectedClearOption]), FocusType.Passive, EditorStyles.toolbarDropDown, GUILayout.Width(80)))
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < clearOptions.Length; i++)
            {
                int index = i;
                menu.AddItem(new GUIContent(clearOptions[i]), selectedClearOption == i, () => 
                {
                    selectedClearOption = index;
                    PerformClearAction(index);
                });
            }
            menu.ShowAsContext();
        }

        // Collapse toggle
        isCollapsed = GUILayout.Toggle(isCollapsed, "Collapse", EditorStyles.toolbarButton);

        // Error Pause toggle
        errorPause = GUILayout.Toggle(errorPause, "Error Pause", EditorStyles.toolbarButton);

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < logEntries.Count; i++)
        {
            var entry = logEntries[i];
            if (isCollapsed && i > 0 && entry.message == logEntries[i - 1].message)
                continue;

            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.normal.textColor = GetLogColor(entry.type);
            style.richText = true;
            style.alignment = TextAnchor.UpperLeft;

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal(GUILayout.Width(15));
            entry.isExpanded = EditorGUILayout.Foldout(entry.isExpanded, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button($"{entry.prefix}[<color=lightblue>{entry.objectName}</color>] {entry.message}", style, GUILayout.ExpandWidth(true)))
            {
                // Handle click event
                if (entry.stackTrace.Contains("UnityEngine.Debug:Log"))
                {
                    JumpToCode(entry.stackTrace);
                }
                else
                {
                    // Highlight the object
                    var obj = GameObject.Find(entry.objectName);
                    if (obj != null)
                    {
                        Selection.activeObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (entry.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Stack Trace:", EditorStyles.boldLabel);
                EditorGUILayout.TextArea(entry.stackTrace, GUILayout.ExpandHeight(true));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }

    private void PerformClearAction(int option)
    {
        switch (option)
        {
            case 0: // Clear
                logEntries.Clear();
                break;
            case 1: // Clear on Play
                EditorApplication.playModeStateChanged += ClearOnPlay;
                break;
            case 2: // Clear on Build
                BuildPlayerWindow.RegisterBuildPlayerHandler(ClearOnBuild);
                break;
        }
        Repaint();
    }

    private void ClearOnPlay(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            logEntries.Clear();
            EditorApplication.playModeStateChanged -= ClearOnPlay;
            Repaint();
        }
    }

    private void ClearOnBuild(BuildPlayerOptions options)
    {
        logEntries.Clear();
        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
        Repaint();
    }
    
    private Color GetLogColor(LogType type)
    {
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                return Color.red;
            case LogType.Warning:
                return Color.yellow;
            default:
                return Color.white;
        }
    }
    private void JumpToCode(string stackTrace)
    {
        var lines = stackTrace.Split('\n');
        string fileName = null;
        int lineNumber = -1;

        foreach (var line in lines)
        {
            if (line.StartsWith("File: "))
                fileName = line.Substring(6).Trim();
            else if (line.StartsWith("Line: "))
                int.TryParse(line.Substring(6).Trim(), out lineNumber);
        }

        if (fileName != null && lineNumber != -1)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Object>(fileName);
            if (asset != null)
            {
                AssetDatabase.OpenAsset(asset, lineNumber);
                return;
            }
        }
        this.LogWarning("Could not find a valid file and line number in the stack trace.");
    }    
    private class LogEntry
    {
        public string prefix;
        public string objectName;
        public string message;
        public string stackTrace;
        public LogType type;
        public bool isExpanded;
        public string fileName;
        public int lineNumber;

        public LogEntry(string prefix, string objectName, string message, string stackTrace, LogType type, string fileName, int lineNumber)
        {
            this.prefix = prefix;
            this.objectName = objectName;
            this.message = message;
            this.stackTrace = stackTrace;
            this.type = type;
            this.isExpanded = false;
            this.fileName = fileName;
            this.lineNumber = lineNumber;
        }
    }

}
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Utils;
using System.Linq;
using System.IO;
using System.Text;

public class FaRConsoleWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private List<LogEntry> logEntries = new List<LogEntry>();
    private string[] clearOptions = new string[] { "Clear", "Clear on Play", "Clear on Build" };
    private int selectedClearOption = 0;
    private bool autoScroll = true;

    private string searchText = "";
    
    private bool showInfo = true;
    private bool showWarning = true;
    private bool showError = true;
    private bool showSuccess = true;

    private bool useRegex = false;
    private string[] searchFields = new string[] { "All", "Message", "Stack Trace", "Object Name" };
    private int selectedSearchField = 0;

    [MenuItem("FARUtils/FaR Console")]
    public static void ShowWindow()
    {
        GetWindow<FaRConsoleWindow>("</> FaR Console");
    }

    static FaRConsoleWindow()
    {
        EditorApplication.delayCall += OpenWindow;
    }

    private static void OpenWindow()
    {
        GetWindow<FaRConsoleWindow>("</> FaR Console");
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        Application.logMessageReceivedThreaded += HandleLogThreaded;
        AssemblyReloadEvents.beforeAssemblyReload += ClearOnRecompile;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
        Application.logMessageReceivedThreaded -= HandleLogThreaded;
        AssemblyReloadEvents.beforeAssemblyReload -= ClearOnRecompile;
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
        var existingEntry = logEntries.FirstOrDefault(e => e.message == message && e.type == type);
        if (existingEntry != null)
        {
            existingEntry.count++;
        }
        else
        {
            string originalFileName = "";
            int originalLineNumber = -1;
            ExtractOriginalFileInfo(stackTrace, out originalFileName, out originalLineNumber);
            logEntries.Add(new LogEntry(prefix, objectName, message, stackTrace, type, originalFileName, originalLineNumber));
        }

        if (autoScroll)
        {
            scrollPosition = new Vector2(0, float.MaxValue);
        }
        Repaint();
    }

    private void ExtractOriginalFileInfo(string stackTrace, out string fileName, out int lineNumber)
    {
        fileName = "";
        lineNumber = -1;
        var lines = stackTrace.Split('\n');
        foreach (var line in lines)
        {
            if (!line.Contains("FaRLogger.cs") && line.Contains("(at "))
            {
                int startIndex = line.LastIndexOf("(at ") + 4;
                int endIndex = line.LastIndexOf(")");
                if (startIndex < endIndex)
                {
                    string fileInfo = line.Substring(startIndex, endIndex - startIndex);
                    string[] parts = fileInfo.Split(':');
                    if (parts.Length >= 2)
                    {
                        fileName = parts[0];
                        int.TryParse(parts[1], out lineNumber);
                        break;
                    }
                }
            }
        }
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
            {
                fileName = line.Substring(6).Trim();
                if (fileName.StartsWith(Application.dataPath))
                {
                    fileName = "Assets" + fileName.Substring(Application.dataPath.Length);
                }
            }
            else if (line.StartsWith("Line: "))
            {
                int.TryParse(line.Substring(6).Trim(), out lineNumber);
            }
        }

        return (prefix, objectName, message, fileName, lineNumber);
    }    
    private bool errorPause = false;
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(50)))
        {
            logEntries.Clear();
        }

        // Add dropdown next to Clear button
        if (EditorGUILayout.DropdownButton(new GUIContent("▼"), FocusType.Passive, EditorStyles.toolbarButton, GUILayout.Width(20)))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Clear on Play"), selectedClearOption == 1, () => 
            {
                selectedClearOption = 1;
                PerformClearAction(1);
            });
            menu.AddItem(new GUIContent("Clear on Build"), selectedClearOption == 2, () => 
            {
                selectedClearOption = 2;
                PerformClearAction(2);
            });
            menu.AddItem(new GUIContent("Clear on Recompile"), selectedClearOption == 3, () => 
            {
                selectedClearOption = 3;
                PerformClearAction(3);
            });
            menu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();
        // Error Pause toggle
        errorPause = GUILayout.Toggle(errorPause, "Error Pause", EditorStyles.toolbarButton);

        // Auto Scroll toggle
        autoScroll = GUILayout.Toggle(autoScroll, "Auto Scroll", EditorStyles.toolbarButton);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Export", EditorStyles.toolbarButton))
        {
            ExportLogsToFile();
        }

        EditorGUILayout.BeginHorizontal();
        searchText = EditorGUILayout.TextField(searchText, EditorStyles.toolbarSearchField);
        useRegex = EditorGUILayout.Toggle(useRegex, GUILayout.Width(20));
        selectedSearchField = EditorGUILayout.Popup(selectedSearchField, searchFields, EditorStyles.toolbarPopup, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        // Toggle buttons for each log type
        showInfo = GUILayout.Toggle(showInfo, new GUIContent("ⓘ", "Info"), EditorStyles.toolbarButton, GUILayout.Width(30));
        showWarning = GUILayout.Toggle(showWarning, new GUIContent("⚠", "Warning"), EditorStyles.toolbarButton, GUILayout.Width(30));
        showError = GUILayout.Toggle(showError, new GUIContent("⌧", "Error"), EditorStyles.toolbarButton, GUILayout.Width(30));
        showSuccess = GUILayout.Toggle(showSuccess, new GUIContent("✓", "Success"), EditorStyles.toolbarButton, GUILayout.Width(30));


        EditorGUILayout.EndHorizontal();        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < logEntries.Count; i++)
        {
            var entry = logEntries[i];
            if (i > 0 && entry.message == logEntries[i - 1].message)
                continue;

            if (!string.IsNullOrEmpty(searchText) && !MatchesSearch(entry))
                continue;

            if (!ShouldShowLogType(entry.entryType))
                continue;

            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.normal.textColor = GetLogColor(entry.type);
            style.richText = true;
            style.alignment = TextAnchor.UpperLeft;

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            // Add time column
            EditorGUILayout.LabelField(entry.timestamp.ToString("HH:mm:ss"), GUILayout.Width(70));

            EditorGUILayout.BeginHorizontal(GUILayout.Width(15));
            entry.isExpanded = EditorGUILayout.Foldout(entry.isExpanded, "");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button($"{entry.prefix}[<color=lightblue>{entry.objectName}</color>] {entry.message}", style, GUILayout.ExpandWidth(true)))
            {
                SmartSelect(entry);
            }

            if (entry.count > 1)
            {
                GUILayout.Label($"({entry.count})", GUILayout.Width(30));
            }

            EditorGUILayout.EndHorizontal();
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

    private void ClearOnRecompile()
    {
        if (selectedClearOption == 3)
        {
            logEntries.Clear();
            Repaint();
        }
    }

    private string GenerateHtmlContent()
    {
        StringBuilder html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang='en'>");
        html.AppendLine("<head>");
        html.AppendLine("<meta charset='UTF-8'>");
        html.AppendLine("<title>FaR Console Log</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial, sans-serif; background-color: #1e1e1e; color: #d4d4d4; }");
        html.AppendLine(".log-entry { border: 1px solid #3c3c3c; margin: 5px; padding: 5px; }");
        html.AppendLine(".log-header { cursor: pointer; }");
        html.AppendLine(".stack-trace { display: none; white-space: pre-wrap; }");
        html.AppendLine(".info { color: #569cd6; }");
        html.AppendLine(".warning { color: #dcdcaa; }");
        html.AppendLine(".error { color: #f44747; }");
        html.AppendLine(".success { color: #6a9955; }");
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        for (int i = 0; i < logEntries.Count; i++)
        {
            var entry = logEntries[i];
            string logClass = entry.entryType.ToString().ToLower();
            html.AppendLine($"<div class='log-entry {logClass}'>");
            html.AppendLine($"<div class='log-header' onclick='toggleStackTrace({i})'>");
            html.AppendLine($"{entry.prefix}[{entry.objectName}] {entry.message}");
            html.AppendLine("</div>");
            html.AppendLine($"<div id='stackTrace{i}' class='stack-trace'>");
            html.AppendLine($"File: {entry.fileName}<br>");
            html.AppendLine($"Line: {entry.lineNumber}<br>");
            html.AppendLine("Stack Trace:<br>");
            html.AppendLine(entry.stackTrace.Replace("\n", "<br>"));
            html.AppendLine("</div>");
            html.AppendLine("</div>");
        }

        html.AppendLine("<script>");
        html.AppendLine("function toggleStackTrace(id) {");
        html.AppendLine("  var stackTrace = document.getElementById('stackTrace' + id);");
        html.AppendLine("  stackTrace.style.display = stackTrace.style.display === 'none' ? 'block' : 'none';");
        html.AppendLine("}");
        html.AppendLine("</script>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }

    private void ExportLogsToFile()
    {
        string path = EditorUtility.SaveFilePanel("Save Console Log", "", "ConsoleLog.html", "html");
        if (!string.IsNullOrEmpty(path))
        {
            string htmlContent = GenerateHtmlContent();
            File.WriteAllText(path, htmlContent);
            this.LogSuccess($"Console log exported to: {path}");
        }
    }

    private bool ShouldShowLogType(LogEntryType type)
    {
        switch (type)
        {
            case LogEntryType.Info:
                return showInfo;
            case LogEntryType.Warning:
                return showWarning;
            case LogEntryType.Error:
                return showError;
            case LogEntryType.Success:
                return showSuccess;
            default:
                return true;
        }
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
            case 3: // Clear on Recompile
                AssemblyReloadEvents.beforeAssemblyReload += ClearOnRecompile;
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

    private bool MatchesSearch(LogEntry entry)
    {
        if (string.IsNullOrEmpty(searchText))
            return true;

        string searchTarget = searchFields[selectedSearchField] switch
        {
            "Message" => entry.message,
            "Stack Trace" => entry.stackTrace,
            "Object Name" => entry.objectName,
            _ => $"{entry.message} {entry.stackTrace} {entry.objectName}"
        };

        if (useRegex)
        {
            try
            {
                return System.Text.RegularExpressions.Regex.IsMatch(searchTarget, searchText, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            catch (System.ArgumentException)
            {
                // Invalid regex, fall back to normal search
                return searchTarget.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }
        else
        {
            return searchTarget.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
    private void SmartSelect(LogEntry entry)
    {
        // First, try to open the file and line number stored in the LogEntry
        if (!string.IsNullOrEmpty(entry.fileName) && entry.lineNumber > 0)
        {
            string relativePath = entry.fileName;
            if (!relativePath.StartsWith("Assets/"))
            {
                relativePath = "Assets/" + relativePath;
            }
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(relativePath);
            if (asset != null)
            {
                Selection.activeObject = asset;
                AssetDatabase.OpenAsset(asset, entry.lineNumber);
                return;
            }
        }

        // If that fails, try to find the GameObject by name
        GameObject obj = GameObject.Find(entry.objectName);
        if (obj != null)
        {
            Selection.activeGameObject = obj;
            EditorGUIUtility.PingObject(obj);
            return;
        }

        // If still not found, parse the stack trace as a last resort
        string[] lines = entry.stackTrace.Split('\n');
        foreach (string line in lines)
        {
            if (!line.Contains("FaRLogger.cs") && line.Contains("(at "))
            {
                int startIndex = line.IndexOf("(at ") + 4;
                int endIndex = line.IndexOf(':', startIndex);
                if (endIndex > startIndex)
                {
                    string filePath = line.Substring(startIndex, endIndex - startIndex);
                    string fullPath = Path.GetFullPath(filePath);
                    string relativePath = "Assets" + fullPath.Substring(Application.dataPath.Length);
            
                    UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(relativePath);
                    if (asset != null)
                    {
                        Selection.activeObject = asset;
                        AssetDatabase.OpenAsset(asset, int.Parse(line.Substring(endIndex + 1).TrimEnd(')')));
                        return;
                    }
                }
            }
        }
    }
    public enum LogEntryType
    {
        Info,
        Warning,
        Error,
        Success
    }
    public class LogEntry
    {
        public string prefix;
        public string objectName;
        public string message;
        public string stackTrace;
        public LogType type;
        public bool isExpanded;
        public string fileName;
        public int lineNumber;
        public int count = 1;
        public LogEntryType entryType;
        public DateTime timestamp;

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
            this.entryType = DetermineLogEntryType(prefix, type);
            this.timestamp = DateTime.Now;
        }

        private LogEntryType DetermineLogEntryType(string prefix, LogType type)
        {
            if (prefix.Contains("✓"))
                return LogEntryType.Success;
            switch (type)
            {
                case LogType.Log:
                    return LogEntryType.Info;
                case LogType.Warning:
                    return LogEntryType.Warning;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    return LogEntryType.Error;
                default:
                    return LogEntryType.Info;
            }
        }
    }
}

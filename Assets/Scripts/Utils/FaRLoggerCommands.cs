using IngameDebugConsole;
using UnityEngine;

namespace Utils
{
    public static class FaRLoggerCommands
    {
        [ConsoleMethod("log.toggle", "Toggle logging for a specific script")]
        public static void ToggleScriptLogging(string scriptName)
        {
            FaRLoggerSettings.ToggleScript(scriptName);
            bool isEnabled = FaRLoggerSettings.GetScriptState(scriptName);
            Debug.Log($"Logging for {scriptName} is now {(isEnabled ? "enabled" : "disabled")}");
        }

        [ConsoleMethod("log.status", "Check if logging is enabled for a script")]
        public static bool GetScriptLoggingStatus(string scriptName)
        {
            return FaRLoggerSettings.GetScriptState(scriptName);
        }
    }
}

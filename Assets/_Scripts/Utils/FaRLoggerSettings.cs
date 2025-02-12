using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class FaRLoggerSettings
    {
        private static HashSet<string> mutedScripts = new HashSet<string>();
        
        public static bool IsScriptMuted(Object context)
        {
            return context != null && mutedScripts.Contains(context.GetType().Name);
        }

        public static void ToggleScript(string scriptName)
        {
            if (mutedScripts.Contains(scriptName))
                mutedScripts.Remove(scriptName);
            else
                mutedScripts.Add(scriptName);
        }

        public static bool GetScriptState(string scriptName)
        {
            return !mutedScripts.Contains(scriptName);
        }
    }
}

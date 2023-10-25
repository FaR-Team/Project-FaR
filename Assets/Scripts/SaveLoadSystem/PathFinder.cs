using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathFinder
{
    public static string GetPath(string finalPath, bool isTemporary)
    {
        string result = Path.Combine(Application.persistentDataPath, GetSaveRunName(),
                            GetTempName(isTemporary), GetCurrentSceneName(), finalPath, ".json"); ;
        return result;
    }

    private static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
    private static string GetSaveRunName()
    {
        return RunName.instance.currentRunName;
    }
    private static string GetTempName(bool isTemporary)
    {
        return (isTemporary ? ".temp" : null);
    }
}

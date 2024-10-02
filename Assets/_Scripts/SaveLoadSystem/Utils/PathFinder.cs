using System.IO;
using UnityEngine;

public static class PathFinder
{
    public static string GetFinalPath(string objectName, bool isTemporary = true)
    {
        string result = GetPathIfTemp(objectName, isTemporary) + ".json";
        return result;
    }

    public static string GetPermanentFolder()
    {
        return Path.Combine(Application.persistentDataPath, GetSaveRunName());
    }
    
    public static string GetTempFolder()
    {
        return Path.Combine(Application.persistentDataPath, "temp");
    }

    private static string GetPathIfTemp(string objectName, bool isTemporary)
    {
        string folder = isTemporary ?
            GetTempFolder() :
            GetPermanentFolder();

        return Path.Combine(folder, objectName);
    }

    private static string GetSaveRunName()
    {
        return RunName.currentRunName;
    }
}
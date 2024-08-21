using System.IO;
using UnityEngine;

public static class SaverManager
{
    public static void Save(object info, bool isTemporary)
    {
        string jsonFile = JsonUtility.ToJson(info);
        string pathFile = PathFinder.GetFinalPath(info.GetType().FullName, isTemporary);
        string directoryPath = Path.GetDirectoryName(pathFile);
        //Debug.Log(directoryPath);
        GUIUtility.systemCopyBuffer = directoryPath;

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(pathFile, jsonFile);
    }

    public static void DeleteSave(object info, bool isTemporary)
    {
        string pathFile = PathFinder.GetFinalPath(info.GetType().FullName, isTemporary);
        if (File.Exists(pathFile))
        {
            File.Delete(pathFile);
            Debug.Log($"Save file deleted: {pathFile}");
        }
        else
        {
            Debug.Log($"No save file found at: {pathFile}");
        }
    }
}

using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

public static class SaverManager
{
    public static DummyLogger logger;

    static SaverManager()
    {
        logger = GameObject.FindObjectOfType<DummyLogger>();
        if (logger == null)
        {
            GameObject loggerObject = new GameObject("DataSaverLogger");
            logger = loggerObject.AddComponent<DummyLogger>();
            UnityEngine.Object.DontDestroyOnLoad(loggerObject);
        }
    }
    public static void Save(object info, bool isTemporary)
    {
        string jsonFile = JsonUtility.ToJson(info);
        string pathFile = PathFinder.GetFinalPath(info.GetType().FullName, isTemporary);
        string directoryPath = Path.GetDirectoryName(pathFile);

        GUIUtility.systemCopyBuffer = directoryPath;

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(pathFile, jsonFile);
        //Debug.Log("Saved temporary in " + pathFile);
        if (!isTemporary)
        {
            pathFile = PathFinder.GetFinalPath(info.GetType().FullName, false);
            File.WriteAllText(pathFile, jsonFile);
        }
    }

    public static void DeleteSave(object info, bool isTemporary)
    {
        string pathFile = PathFinder.GetFinalPath(info.GetType().FullName, isTemporary);
        if (File.Exists(pathFile))
        {
            File.Delete(pathFile);
            logger.Log($"Save file deleted: {pathFile}");
        }
        else
        {
            logger.Log($"No save file found at: {pathFile}");
        }
    }

    public static void DeleteAllSaves()
    {
        string folderPath = PathFinder.GetPermanentFolder();

        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                File.Delete(file);
                logger.Log($"Save file deleted: {file}");
            }

            logger.Log($"All save files deleted in folder: {folderPath}");
        }
        else
        {
            logger.Log($"No save folder found at: {folderPath}");
        }
    }
    public static void ClearTemp()
    {
        try
        {
            Directory.Delete(PathFinder.GetTempFolder(), true);
        }
        catch (Exception e)
        {
            logger.LogWarning(e);
        }
    }
}
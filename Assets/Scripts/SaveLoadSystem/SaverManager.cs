﻿using System.IO;
using UnityEngine;

public static class SaverManager 
{
    public static void Save(object info, bool isTemporary)
    {
        string jsonFile = JsonUtility.ToJson(info);
        string pathFile = PathFinder.GetPath(info.GetType().FullName, isTemporary);
        string directoryPath = Path.GetDirectoryName(pathFile);
        //Debug.Log(directoryPath);
        GUIUtility.systemCopyBuffer = directoryPath;
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(pathFile, jsonFile);
    }
}
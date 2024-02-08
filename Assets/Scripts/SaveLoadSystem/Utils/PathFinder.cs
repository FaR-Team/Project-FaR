﻿using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PathFinder
{
    public static string GetPath(string finalPath, bool isTemporary)
    {
        string result = GetPathIfTemp(finalPath, isTemporary) + ".json";
        return result;
    }

    private static string GetPathIfTemp(string finalPath, bool isTemporary)
    {
        if (isTemporary)
        {
            return Path.Combine(Application.persistentDataPath, GetSaveRunName(),
                                ".temp", GetCurrentSceneName(), finalPath);
        }
        else
        {
            return Path.Combine(Application.persistentDataPath, "Run Test", GetCurrentSceneName(), finalPath);
        }
    }

    private static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
    private static string GetSaveRunName()
    {
        return RunName.instance.currentRunName;
    }

}
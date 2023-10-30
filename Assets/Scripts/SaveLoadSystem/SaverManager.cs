using System;
using System.IO;
using UnityEngine;

public class SaverManager : MonoBehaviour
{
    SaverManager instance;
    private void Awake()
    {
        if (instance != null) Destroy(instance);

        instance = this;
    }

    public static void Save(object info, bool isTemporary)
    {
        string jsonFile = JsonUtility.ToJson(info);
        string pathFile = PathFinder.GetPath(info.GetType().FullName, isTemporary);
        string directoryPath = Path.GetDirectoryName(pathFile);
        Debug.Log(directoryPath);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(pathFile, jsonFile);
    }
}

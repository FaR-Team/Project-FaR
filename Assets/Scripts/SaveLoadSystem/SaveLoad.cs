using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;


public static class SaveLoad
{
    public static UnityAction OnSaveGame;
    public static UnityAction<SaveData> OnLoadGame;

    private static string directory = "/SaveData/";
    private static string fileName = "SaveGame.sav";

    public static bool Save(SaveData data)
    {
        OnSaveGame?.Invoke();

        string dir = Application.persistentDataPath + directory;
    
        //GUIUtility.systemCopyBuffer = dir;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    
        string Json = JsonUtility.ToJson(data, true);
        File.WriteAllText(dir + fileName, Json);

        Debug.Log("Guardando...");

        return true;
    }

    public static SaveData load()
    {
        string FullPath = Application.persistentDataPath + directory + fileName;
        SaveData data = new SaveData();

        if (File.Exists(FullPath))
        {
            string Json = File.ReadAllText(FullPath);
            data = JsonUtility.FromJson<SaveData>(Json);

            OnLoadGame?.Invoke(data);
        }
        else
        {
            Debug.Log("No existe archivo de Guardado");
        }

        return data;
    }

    public static void DeleteSaveData()
    {
        string FullPath = Application.persistentDataPath + directory + fileName;
        if (File.Exists(FullPath))
        {
            File.Delete(FullPath);
        }
    }
}

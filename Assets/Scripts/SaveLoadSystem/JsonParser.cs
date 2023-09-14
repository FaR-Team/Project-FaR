using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonParser
{
    public static Dictionary<string, IPersistent> keyValuePairs;

    string savePath;

    public JsonParser(string savePath, Dictionary<string, IPersistent> keyValuePairs)
    {
        this.savePath = Application.persistentDataPath + savePath;
        string jsonData = JsonUtility.ToJson(keyValuePairs);
        File.WriteAllText(savePath, jsonData);
        this.savePath = savePath;
    }

    public void SaveGame(SaveData saveData)
    {
        string jsonData = JsonUtility.ToJson(saveData);
        File.WriteAllText(savePath, jsonData);
    }
}
using System;
using System.IO;
using UnityEngine;

public class Loader
{
    public static object Load(string filePath)
    {
        object result;
        try
        {
            string fileContent = File.ReadAllText(filePath);
            result = JsonUtility.FromJson<object>(fileContent);
        }
        catch (FileNotFoundException ex)
        {
            result = null;
            Debug.LogWarning(ex.Message);
        }
        return null;
    }
}

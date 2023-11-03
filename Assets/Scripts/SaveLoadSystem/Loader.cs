using System;
using System.IO;
using UnityEngine;

public class Loader<T>
{
    public T Load(string filePath)
    {
        try
        {
            return TryLoad(filePath);
        }
        catch
        {
            throw;
        }
    }

    private T TryLoad(string filePath)
    {
        string fileContent = File.ReadAllText(filePath);
        var result = JsonUtility.FromJson<T>(fileContent);
        return result;
    }
}

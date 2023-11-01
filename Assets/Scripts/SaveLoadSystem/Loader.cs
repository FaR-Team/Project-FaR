using System;
using System.IO;
using UnityEngine;

public class Loader<T>
{
    public T Load(string filePath)
    {
        try
        {
            string fileContent = File.ReadAllText(filePath);
            var result = JsonUtility.FromJson<T>(fileContent);
            return result;
        }
        catch
        {
            throw new Exception("NO EXISTE ESTE ARCHIVO");
        }
    }
}

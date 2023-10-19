using System.IO;
using UnityEngine;

public class Loader
{
    public static object Load(string filePath)
    {
        return JsonUtility.FromJson<object>(File.ReadAllText(filePath));
    }
}

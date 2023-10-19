using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ParserData
{
    public static void Save(object info)
    {
      //  string jsonData = JsonUtility.ToJson(info);
       // File.WriteAllText(info, jsonData);
    }

    public static object Load()
    {
        return null;
    }
}
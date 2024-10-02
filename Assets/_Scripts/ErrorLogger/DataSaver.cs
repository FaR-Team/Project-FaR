using System;
using System.IO;
using System.Text;
using UnityEngine;
using Utils;

public class DataSaver
{
    public static DummyLogger logger;

    static DataSaver()
    {
        logger = GameObject.FindObjectOfType<DummyLogger>();
        if (logger == null)
        {
            GameObject loggerObject = new GameObject("DataSaverLogger");
            logger = loggerObject.AddComponent<DummyLogger>();
            UnityEngine.Object.DontDestroyOnLoad(loggerObject);
        }
    }

    public static void SaveData<T>(T dataToSave, string dataFileName)
    {
        string tempPath = Path.Combine(Application.persistentDataPath, "data");
        tempPath = Path.Combine(tempPath, dataFileName + ".txt");

        //Convertir a Json y después a bytes
        string jsonData = JsonUtility.ToJson(dataToSave, true);
        byte[] jsonByte = Encoding.ASCII.GetBytes(jsonData);

        //Crear el directorio si no existe
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
        }

        try
        {
            File.WriteAllBytes(tempPath, jsonByte);
            logger.Log("Se guardaron los datos en: " + tempPath.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            logger.LogWarning("Fallo al guardar los datos en: " + tempPath.Replace("/", "\\"));
            logger.LogWarning("Error: " + e.Message);
        }
    }

    //Cargar
    public static T LoadData<T>(string dataFileName)
    {
        string tempPath = Path.Combine(Application.persistentDataPath, "data");
        tempPath = Path.Combine(tempPath, dataFileName + ".txt");

        //Salir si el directorio o archivo no existe.
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            logger.LogWarning("El directorio no existe");
            return default;
        }

        if (!File.Exists(tempPath))
        {
            logger.LogWarning("El archivo no existe");
            return default;
        }

        //Cargar el Json
        byte[] jsonByte = null;
        try
        {
            jsonByte = File.ReadAllBytes(tempPath);
            logger.Log("Se cargó desde: " + tempPath.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            logger.LogWarning("Fallo al cargar los datos desde: " + tempPath.Replace("/", "\\"));
            logger.LogWarning("Error: " + e.Message);
        }

        string jsonData = Encoding.ASCII.GetString(jsonByte);

        //Convert a Objeto
        object resultValue = JsonUtility.FromJson<T>(jsonData);
        return (T)Convert.ChangeType(resultValue, typeof(T));
    }

    private static void fault<T>(T t)
    {
        throw new NotImplementedException();
    }

    public static bool DeleteData(string dataFileName)
    {
        bool success = false;

        string tempPath = Path.Combine(Application.persistentDataPath, "data");
        tempPath = Path.Combine(tempPath, dataFileName + ".txt");

        //Salir si el directorio o archivo no existe.
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            logger.LogWarning("El directorio no existe");
            return false;
        }

        if (!File.Exists(tempPath))
        {
            logger.Log("El archivo no existe");
            return false;
        }

        try
        {
            File.Delete(tempPath);
            logger.Log("Se eliminaron datos de: " + tempPath.Replace("/", "\\"));
            success = true;
        }
        catch (Exception e)
        {
            logger.LogWarning("Fallo al eliminar datos de: " + e.Message);
        }

        return success;
    }
}
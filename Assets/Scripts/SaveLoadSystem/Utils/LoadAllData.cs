using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

public static class LoadAllData
{

    public static DummyLogger logger;

    static LoadAllData()
    {
        logger = GameObject.FindObjectOfType<DummyLogger>();
        if (logger == null)
        {
            GameObject loggerObject = new GameObject("DataSaverLogger");
            logger = loggerObject.AddComponent<DummyLogger>();
            UnityEngine.Object.DontDestroyOnLoad(loggerObject);
        }
    }

    public static T GetData<T>(bool temporary) where T : IAllData<T>, new()
    {
        T data = new();
        string path = PathFinder.GetFinalPath(data.GetType().FullName, temporary);

        //Debug.Log("Tried loading from path " + path);

        if (!PathExists(path))
        {
            Initialize();
        }

        try
        {
            T result = new();

            string fileContent = File.ReadAllText(path);
            T preresult = JsonUtility.FromJson<T>(fileContent);

            result.CopyData(preresult);

            return result;
        }
        catch
        {
            throw; // Return default value for error case
        }
    }
    private static bool PathExists(string path)
    {
        return Directory.Exists(path) || File.Exists(path);
    }
    private static void Initialize()
    {
        CopyFolderToTemp();
    }

    private static void CopyFolderToTemp()
    {
        try
        {
            string tempDirectory = PathFinder.GetTempFolder();
            string sourceFolderPath = PathFinder.GetPermanentFolder();

            if (!Directory.Exists(sourceFolderPath)) return;

            // Crear la carpeta temp si no existe
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
                File.SetAttributes(tempDirectory, FileAttributes.Hidden);
            }

            // Copiar el contenido de la carpeta
            CopyDirectoryContents(sourceFolderPath, tempDirectory);

            logger.Log($"El contenido de la carpeta ha sido copiado a: {tempDirectory}");
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning($"Error de permisos: {ex.Message}");
        }
        catch (IOException ex)
        {
            logger.LogWarning($"Error de entrada/salida: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Error inesperado: {ex.Message}");
        }
    }

    static void CopyDirectoryContents(string sourceDir, string destDir)
    {
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            string destinyPath = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destinyPath, true);
        }

        foreach (var directory in Directory.GetDirectories(sourceDir))
        {
            CopyDirectoryContents(directory, destDir);
        }
    }
}

public interface IAllData<T>
{
    public void CopyData(T data);
}
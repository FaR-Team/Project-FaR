using System;
using System.IO;
using UnityEngine;

public static class LoadAllData
{
    public static T GetData<T>() where T : IAllData<T>, new()
    {
        T data = new();
        string path = PathFinder.GetFinalPath(data.GetType().FullName, true);

        if (PathExists(path))
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
        string tempDirectory = PathFinder.GetTempFolder();
        string sourceFolderPath = PathFinder.GetPermanentFolder();

        if (!Directory.Exists(sourceFolderPath)) return;

        // Crear la carpeta temp si no existe
        if (!Directory.Exists(tempDirectory))
        {
            Directory.CreateDirectory(tempDirectory);
            File.SetAttributes(tempDirectory, FileAttributes.Hidden);
        }

        string destinationFolderPath = PathFinder.GetTempFolder();

        // Copiar la carpeta
        CopyDirectory(sourceFolderPath, destinationFolderPath);

        Console.WriteLine($"La carpeta ha sido copiada a: {destinationFolderPath}");
    }

    static void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            string destinyPath = PathFinder.GetFinalPath(Path.GetFileName(file));
            File.Copy(file, destinyPath, true);
        }

        foreach (var directory in Directory.GetDirectories(sourceDir))
        {
            string destDirectory = Path.Combine(destDir, Path.GetFileName(directory));
            CopyDirectory(directory, destDirectory);
        }
    }
}

public interface IAllData<T>
{
    public void CopyData(T data);
}
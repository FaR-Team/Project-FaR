using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using SimpleFileBrowser;

public static class FaREditorUtils 
{

    public static void SaveScreenshot(string timestamp)
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".png"));
        FileBrowser.SetDefaultFilter(".png");
        
        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string defaultName = $"screenshot_{timestamp}.png";

        FileBrowser.ShowSaveDialog((paths) => {
            string saveFilePath = paths[0];
            ScreenCapture.CaptureScreenshot(saveFilePath);
            Debug.Log($"Screenshot saved to: {saveFilePath}");
        },
        () => {
            Debug.Log("Screenshot save cancelled");
        },
        FileBrowser.PickMode.Files,
        false,
        defaultPath,
        defaultName,
        "Save Screenshot",
        "Save");
    }

    public static void ExportData(string timestamp, string tempExportPath, string logContent)
    {
        Directory.CreateDirectory(tempExportPath);

        // Export logs
        File.WriteAllText(Path.Combine(tempExportPath, "debug_logs.txt"), logContent);

        // Copy all save files
        string savesDirectory = PathFinder.GetPermanentFolder();
        if (Directory.Exists(savesDirectory))
        {
            string savesFolderInZip = Path.Combine(tempExportPath, "saves");
            Directory.CreateDirectory(savesFolderInZip);
            foreach (string file in Directory.GetFiles(savesDirectory))
            {
                File.Copy(file, Path.Combine(savesFolderInZip, Path.GetFileName(file)));
            }
        }

        FileBrowser.SetFilters(true, new FileBrowser.Filter("ZIP files", ".zip"));
        FileBrowser.SetDefaultFilter(".zip");

        FileBrowser.ShowSaveDialog((paths) => {
            string saveFilePath = paths[0];
            if (File.Exists(saveFilePath))
                File.Delete(saveFilePath);
                
            System.IO.Compression.ZipFile.CreateFromDirectory(tempExportPath, saveFilePath);
            Debug.Log($"Data exported to: {saveFilePath}");

            // Cleanup temp directory
            if (Directory.Exists(tempExportPath))
                Directory.Delete(tempExportPath, true);
        },
        () => {
            Debug.Log("Export cancelled");
            // Cleanup temp directory on cancel
            if (Directory.Exists(tempExportPath))
                Directory.Delete(tempExportPath, true);
        },
        FileBrowser.PickMode.Files,
        false,
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        $"FaR_Export_{timestamp}.zip",
        "Save Export Data",
        "Save");
    }
}

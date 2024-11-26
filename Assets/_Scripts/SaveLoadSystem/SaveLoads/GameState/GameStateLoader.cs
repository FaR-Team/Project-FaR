using System;
using FaRUtils.Systems.DateTime;
using UnityEngine;
using Utils;

public static class GameStateLoader
{
    public static DummyLogger logger;
    
    static GameStateLoader()
    {
        logger = GameObject.FindObjectOfType<DummyLogger>();
        if (logger == null)
        {
            GameObject loggerObject = new GameObject("DataSaverLogger");
            logger = loggerObject.AddComponent<DummyLogger>();
            UnityEngine.Object.DontDestroyOnLoad(loggerObject);
        }
    }

    public static GameStateData gameStateData;
    
    public static GameStateData Load(bool temporary)
    {
        return TryGetGameStateData(temporary);
    }
    private static GameStateData TryGetGameStateData(bool temporary)
    {
        try
        {
            gameStateData = LoadAllData.GetData<GameStateData>(temporary);

            return gameStateData;
        }
        catch (Exception e)
        {
            logger.LogWarning($"Failed to load Game State. reason {e}");

            return new GameStateData(TimeManager.DateTime); // Return default time TODO: Revisar si está bien esto, pero para testing por ahora
        }
    }
}

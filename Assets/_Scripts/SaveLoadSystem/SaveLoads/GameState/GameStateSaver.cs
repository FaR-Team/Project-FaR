using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FaRUtils.Systems.DateTime;
using UnityEngine;
using Utils;
using UnityEngine.SceneManagement;
using DateTime = FaRUtils.Systems.DateTime.DateTime;

public class GameStateSaver : Saver<GameStateData, GameStateDataSaver>
{
    public static GameStateSaver Instance;
    private GameStateData gameStateData;
    private GameStateDataSaver gameStateDataSaver;
    
    public GameStateData CurrentGameStateData => gameStateData;

    private void Awake()
    {
        Instance = this;
    }
    
    public override void AddSavedObject(GameStateDataSaver y)
    {
        gameStateDataSaver = y;
    }

    public override void RemoveSavedObject(GameStateDataSaver y)
    {
        gameStateDataSaver = null;
    }

    public override Task WriteSave(GameStateData t)
    {
        gameStateData = t;

        return Task.CompletedTask;
    }

    protected override async void SaveAllData(bool isTemporarySave)
    {
        try
        {
            await SaveGameState();
            
            SaverManager.Save(gameStateData, isTemporarySave);

            //this.LogSuccess("Successfully Saved Game State");
        }
        catch (Exception e)
        {
            this.LogError($"Failed Save Game State. Reason: {e}");
        }
    }
    private async Task SaveGameState()
    {
        await gameStateDataSaver.SaveData();
    }
}

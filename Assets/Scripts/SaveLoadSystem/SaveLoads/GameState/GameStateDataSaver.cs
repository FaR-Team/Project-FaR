using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FaRUtils.Systems.DateTime;
using UnityEngine;

public class GameStateDataSaver : DataSaver<TimeManager, GameStateDataSaver>, IDataSavable
{
    [SerializeField] GameStateSaver saver;

    protected override void SetThisInstance()
    {
        thisDataSaver = this;
        saver = GameStateSaver.Instance;
        saverAllData = saver;
    }

    public override async Task SaveData()
    {
        var sellCart = SellSystem.Instance != null ? SellSystem.Instance.ShoppingCart : new List<ShoppingCartItem>();
        GameStateData gameStateData = new(TimeManager.DateTime, TimeManager.Instance.SceneStates, 
            new SellSystemData(sellCart), 
            new PlayerStatsData(PlayerStats.Instance));

        await saverAllData.WriteSave(gameStateData);
    }
}

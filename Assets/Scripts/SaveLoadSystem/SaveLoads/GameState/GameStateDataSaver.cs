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
        List<ShoppingCartItem> sellCart;
        if (SellSystem.Instance != null)
        {
            sellCart = SellSystem.Instance.ShoppingCart;
        }
        else
        {
            var existingState = LoadAllData.GetData<GameStateData>(true);
            sellCart = existingState?.SellSystemData?.shoppingCart != null 
                ? new List<ShoppingCartItem>(existingState.SellSystemData.shoppingCart) 
                : new List<ShoppingCartItem>();
        }

        GameStateData gameStateData = new(TimeManager.DateTime, TimeManager.Instance.SceneStates, 
            new SellSystemData(sellCart), 
            new PlayerStatsData(PlayerStats.Instance));

        await saverAllData.WriteSave(gameStateData);
    }
}

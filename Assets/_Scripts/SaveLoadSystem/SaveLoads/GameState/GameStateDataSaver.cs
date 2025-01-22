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
        GameStateData gameStateData = new(TimeManager.DateTime, TimeManager.Instance.SceneStates, new SellSystemData(SellSystem.ShoppingCart));

        await saverAllData.WriteSave(gameStateData);
    }
}

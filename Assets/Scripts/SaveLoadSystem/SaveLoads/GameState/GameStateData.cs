using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DateTime = FaRUtils.Systems.DateTime.DateTime;

[System.Serializable]
public class GameStateData : SaveData, IAllData<GameStateData>
{
    [SerializeField] DateTime _currentDateTime;
    [SerializeField] List<SceneStateData> _sceneStates;
    [SerializeField] SellSystemData _sellSystemData;
    public DateTime CurrentDateTime => _currentDateTime;
    public List<SceneStateData> SceneStates => _sceneStates;
    public SellSystemData SellSystemData => _sellSystemData;

    public GameStateData()
    {
        _currentDateTime = new DateTime();
        _sceneStates = new List<SceneStateData>();
        _sellSystemData = new SellSystemData();
    }
    public GameStateData(DateTime time)
    {
        _currentDateTime = time;
        _sceneStates = new List<SceneStateData>();
        _sellSystemData = new SellSystemData();
    }
    
    public GameStateData(DateTime time, List<SceneStateData> sceneStates, SellSystemData sellSystemData)
    {
        _currentDateTime = time;
        _sceneStates = sceneStates;
        _sellSystemData = sellSystemData;
    }
    public void CopyData(GameStateData data)
    {
        this._currentDateTime = data._currentDateTime;
        this._sceneStates = data._sceneStates;
        this._sellSystemData = data._sellSystemData;
    }
}

[Serializable]
public struct SceneStateData
{
    public string sceneName;
    public DateTime lastDateTime;
}

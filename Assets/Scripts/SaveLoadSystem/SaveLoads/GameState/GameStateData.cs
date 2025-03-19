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
    [SerializeField] PlayerStatsData _playerStatsData;
    public DateTime CurrentDateTime => _currentDateTime;
    public List<SceneStateData> SceneStates => _sceneStates;
    public SellSystemData SellSystemData => _sellSystemData;
    public PlayerStatsData PlayerStatsData => _playerStatsData; 

    public GameStateData()
    {
        _currentDateTime = new DateTime();
        _sceneStates = new List<SceneStateData>();
        _sellSystemData = new SellSystemData();
        _playerStatsData = new PlayerStatsData();
    }
    public GameStateData(DateTime time)
    {
        _currentDateTime = time;
        _sceneStates = new List<SceneStateData>();
        _sellSystemData = new SellSystemData();
        _playerStatsData = new PlayerStatsData();
    }
    
    public GameStateData(DateTime time, List<SceneStateData> sceneStates, SellSystemData sellSystemData, PlayerStatsData playerStatsData)
    {
        _currentDateTime = time;
        _sceneStates = sceneStates;
        _sellSystemData = sellSystemData;
        _playerStatsData = playerStatsData;
    }
    public void CopyData(GameStateData data)
    {
        this._currentDateTime = data._currentDateTime;
        this._sceneStates = data._sceneStates;
        this._sellSystemData = data._sellSystemData;
        this._playerStatsData = data._playerStatsData;
    }
}

[Serializable]
public struct SceneStateData
{
    public string sceneName;
    public DateTime lastDateTime;
}

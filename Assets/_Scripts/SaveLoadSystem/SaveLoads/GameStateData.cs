using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DateTime = FaRUtils.Systems.DateTime.DateTime;

[System.Serializable]
public class GameStateData : SaveData, IAllData<GameStateData>
{
    [SerializeField] DateTime _currentDateTime;
    [SerializeField] DateTime _lastSaveDateTime;
    [SerializeField] List<SceneStateData> _sceneStates;
    public DateTime CurrentDateTime => _currentDateTime;
    public DateTime LastSaveDateTime => _lastSaveDateTime;
    public List<SceneStateData> SceneStates => _sceneStates;

    public GameStateData()
    {
        _currentDateTime = new DateTime();
        _lastSaveDateTime = new DateTime();
        _sceneStates = new List<SceneStateData>();
    }
    public GameStateData(DateTime time)
    {
        _currentDateTime = time;
        _lastSaveDateTime = time;
        _sceneStates = new List<SceneStateData>();
    }
    
    public GameStateData(DateTime time, List<SceneStateData> sceneStates)
    {
        _currentDateTime = time;
        _lastSaveDateTime = time;
        _sceneStates = new List<SceneStateData>();
    }
    public void CopyData(GameStateData data)
    {
        this._currentDateTime = data._currentDateTime;
        this._lastSaveDateTime = data._lastSaveDateTime;
        this._sceneStates = data._sceneStates;
    }
    public void SetLastSaveDateTime(DateTime time)
    {
        this._lastSaveDateTime = time; 
    }

    
}

[Serializable]
public struct SceneStateData
{
    public string sceneName;
    public DateTime lastDateTime;
}

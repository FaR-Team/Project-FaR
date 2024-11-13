using FaRUtils.Systems.DateTime;
using UnityEngine;

[System.Serializable]
public class GameStateData : SaveData, IAllData<GameStateData>
{
    [SerializeField] DateTime _currentDateTime;
    [SerializeField] DateTime _lastSaveDateTime;
    public DateTime CurrentDateTime => _currentDateTime;
    public DateTime LastSaveDateTime => _lastSaveDateTime;

    public GameStateData()
    {
        _currentDateTime = new DateTime();
    }
    public GameStateData(DateTime time)
    {
        _currentDateTime = time;
        _lastSaveDateTime = time;
    }
    public void CopyData(GameStateData data)
    {
        this._currentDateTime = data._currentDateTime;
        this._lastSaveDateTime = data._lastSaveDateTime;
    }
    public void SetLastSaveDateTime(DateTime time)
    {
        this._lastSaveDateTime = time; 
    }
}

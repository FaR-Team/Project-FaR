using System.Collections;
using System.Collections.Generic;
using FaRUtils.Systems.DateTime;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

[System.Serializable]
public class GameStateData : SaveData, IAllData<GameStateData>
{
    [SerializeField] DateTime _currentDateTime;
    public DateTime CurrentDateTime => _currentDateTime;

    public GameStateData()
    {
        _currentDateTime = new DateTime();
    }
    public GameStateData(DateTime time)
    {
        _currentDateTime = time;
    }
    public void CopyData(GameStateData data)
    {
        this._currentDateTime = data._currentDateTime;
    }
}

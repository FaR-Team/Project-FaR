using System;
using System.Collections;
using System.Collections.Generic;
using FaRUtils.Systems.DateTime;
using UnityEngine;
using Utils;

public class CatchUpBroadcaster : MonoBehaviour
{
    public event Action<int> OnCatchUpBroadcast;
    
    public static CatchUpBroadcaster Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void OnEnable()
    {
        DirtSpawnerPooling.OnAllDirtsLoaded += CatchUp;
    }
    
    private void OnDisable()
    {
        DirtSpawnerPooling.OnAllDirtsLoaded -= CatchUp;
    }

    void CatchUp()
    {
        // Calculate days between current time and last save
        var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var currentTime = TimeManager.DateTime;
        var lastSaveTime = TimeManager.Instance.GetLastTimeInScene(sceneName);

        var currentDayCount = currentTime.Hour < 3 ? currentTime.TotalNumDays - 1 : currentTime.TotalNumDays;
        var lastDayCount = lastSaveTime.Hour < 3 ? lastSaveTime.TotalNumDays - 1 : lastSaveTime.TotalNumDays;
        var daysPassed = currentDayCount - lastDayCount;
        
        this.Log($"Days passed: {daysPassed}", $"Current time: {currentTime.Date}", $"Last save time: {lastSaveTime.Date}");
        
        OnCatchUpBroadcast?.Invoke(daysPassed);
        
        TimeManager.Instance.AddSceneLastTime(sceneName, TimeManager.DateTime);
    }
}

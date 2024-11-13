using UnityEngine;
using System;
using DateTime = FaRUtils.Systems.DateTime.DateTime;

public class GrowthEventManager : MonoBehaviour
{
    public static GrowthEventManager Instance { get; private set; }
    
    public event Action<int> OnHourChanged;
    public event Action<GrowingBase, GrowingState> OnGrowthStateChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DateTime.OnHourChanged.AddListener(BroadcastHourChange);
    }

    private void BroadcastHourChange(int hour)
    {
        OnHourChanged?.Invoke(hour);
    }

    public void NotifyGrowthStateChanged(GrowingBase plant, GrowingState newState)
    {
        OnGrowthStateChanged?.Invoke(plant, newState);
    }

    private void OnDestroy()
    {
        DateTime.OnHourChanged.RemoveListener(BroadcastHourChange);
    }
}

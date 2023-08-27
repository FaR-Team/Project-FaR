using System;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    [Header("Manejo del Clima")]
    [SerializeField] private int weatherQueueSize = 5;
    [SerializeField] private Weather currentWeather = Weather.Sunny;
    public Weather CurrentWeather => currentWeather;
    private Queue<Weather> weatherQueue;

    [Header("Efectos del Clima")]
    [SerializeField] ParticleSystem rainParticles;
    [SerializeField] ParticleSystem snowParticles;
    [SerializeField] ParticleSystem cloudParticles;

    [Header("Debug")]
    public bool forceRain = false;

    public static Action <Weather, Queue<Weather>> OnWeatherChanged;

    private void CheckHourChangedForWeatherChange(int hour)
    {
        if (hour != 6) return;
        ChangeWeather();
    }

    private void Start()
    {
        rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        FaRUtils.Systems.DateTime.DateTime.OnHourChanged.AddListener(CheckHourChangedForWeatherChange);

        FillWeatherQueue();
        ChangeWeather();
    }

    private void FillWeatherQueue()
    {
        weatherQueue = new Queue<Weather>();

        for (int i = 0; i < weatherQueueSize; i++)
        {
            Weather tempWeather = GetRandomWeather();
            weatherQueue.Enqueue(tempWeather);
            Debug.Log($"{tempWeather} en el index {i}");
        }
    }

    private Weather GetRandomWeather()
    {
        int randomWeather = 0;

        if (!forceRain)
        {
            randomWeather = UnityEngine.Random.Range(0, (int)Weather.WEATHER_MAX + 1);
        }

        return (Weather)randomWeather;
    }

    void ChangeWeather()
    {
        currentWeather = weatherQueue.Dequeue();
        weatherQueue.Enqueue(GetRandomWeather());

        OnWeatherChanged?.Invoke(currentWeather, weatherQueue);

        switch (currentWeather)
        {
            case Weather.Sunny:
                rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                break;
            case Weather.Cloudy:
                rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                cloudParticles.Play();
                break;
            case Weather.Rain:
                rainParticles.Play();
                snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                break;
            case Weather.Snow:
                rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                snowParticles.Play();
                cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                break;
            default:
                break;
        }
    }

    
}

public enum Weather
{
    Sunny = 0,
    Cloudy = 1,
    Rain = 2,
    Snow = 3,
    WEATHER_MAX = Snow
}

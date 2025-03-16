using System.Collections.Generic;
using FaRUtils.Systems.DateTime;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace FaRUtils.Systems.Weather
{
    public class WeatherManager : MonoBehaviour
    {
        public static WeatherManager Instance { get; private set; }

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

        public static UnityEvent<Weather, Queue<Weather>> OnWeatherChanged;
        public UnityEvent IsRaining;
        public static UnityEvent IsSnowing;
        public static UnityEvent IsCloudy;

        void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this; 
            } 
        }

        private void Start()
        {
            rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            FaRUtils.Systems.DateTime.DateTime.OnHourChanged.AddListener(CheckHourChangedForWeatherChange);
            FaRUtils.Systems.DateTime.DateTime.OnSeasonChanged.AddListener(HandleSeasonChange);

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
            }
        }
        private void HandleSeasonChange(DateTime.DateTime.Season newSeason)
        {
            FillWeatherQueue();
            ChangeWeather();
        }

        private Weather GetRandomWeather()
        {
            if (forceRain)
                return Weather.Rain;

            var currentSeason = TimeManager.DateTime.Seasons;

            switch (currentSeason)
            {
                case FaRUtils.Systems.DateTime.DateTime.Season.Early_Spring:
                case FaRUtils.Systems.DateTime.DateTime.Season.Late_Spring:
                    return (Weather)UnityEngine.Random.Range(0, 3); // Sunny, Cloudy, Rain
                    
                case FaRUtils.Systems.DateTime.DateTime.Season.Early_Summer:
                case FaRUtils.Systems.DateTime.DateTime.Season.Late_Summer:
                    return (Weather)UnityEngine.Random.Range(0, 2); // Sunny, Cloudy
                    
                case FaRUtils.Systems.DateTime.DateTime.Season.Early_Fall:
                case FaRUtils.Systems.DateTime.DateTime.Season.Late_Fall:
                    return (Weather)UnityEngine.Random.Range(0, 3); // Sunny, Cloudy, Rain
                    
                case FaRUtils.Systems.DateTime.DateTime.Season.Early_Winter:
                case FaRUtils.Systems.DateTime.DateTime.Season.Late_Winter:
                    var winterWeather = UnityEngine.Random.Range(0, 4);
                    return winterWeather == 3 ? Weather.Snow : (Weather)winterWeather; // Sunny, Cloudy, Rain, Snow
                    
                default:
                    return Weather.Sunny;
            }
        }

        void ChangeWeather()
        {
            currentWeather = weatherQueue.Dequeue();
            weatherQueue.Enqueue(GetRandomWeather());

            OnWeatherChanged?.Invoke(currentWeather, weatherQueue);
            
            if (rainParticles != null || snowParticles != null || cloudParticles != null)
            {
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
                        IsCloudy?.Invoke();
                        break;
                    case Weather.Rain:
                        rainParticles.Play();
                        snowParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                        cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                        IsRaining?.Invoke();
                        break;
                    case Weather.Snow:
                        rainParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                        snowParticles.Play();
                        cloudParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                        IsSnowing?.Invoke();
                        break;
                    default:
                        break;
                }
            }
        }

        private void CheckHourChangedForWeatherChange(int hour)
        {
            if (hour != 6) return;
            ChangeWeather();
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
}
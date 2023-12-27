using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FaRUtils.Systems.DateTime;

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
        public TimeManager timeManager;

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
                Debug.Log($"{tempWeather} on index {i}");
            }
        }

        private Weather GetRandomWeather()
        {
            int randomWeather = 0;

            if (!forceRain)
            {
                //Pasarle la season a un objeto para que te diga qué weathers pueden ocurrir, quizá con SO, no sé aaaaaa
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

using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using Utils;

namespace FaRUtils.Systems.DateTime
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }
        
        [SerializeField] List<SceneStateData> _sceneStates = new();

        [Header("Opciones de Fecha y Tiempo")]

        [Range(1, 28), Header("Dia del Mes")]
        public int dateInMonth;

        [Range(1, 8), Header("Época del Año")]
        public int season;

        [Range(1, 99), Header("Año")]
        public int year;

        [Range(0, 24), Header("Hora")]
        public int hour;

        [Range(0, 6), Header("Minuto")]
        public int minutes;

        [OdinSerialize]
        public static DateTime DateTime;

        [Header("Opciones de Tiempo")]

        public int TickMinutesIncreased = 10;
        public static float TimeBetweenTicks = 10f;
        private float CurrentTimeBetweenTicks = 0;
        
        public List<SceneStateData> SceneStates => _sceneStates;

        public static LocalizedString localizedStringClock;

        public static UnityAction<DateTime> OnDateTimeChanged;

        public static UnityEvent<int> _OnHourChanged => DateTime.OnHourChanged;

        private void Awake()
        {
            SetInstance();
        }

        private void SetInstance()
        {
            if (Instance == null)
            {
                Instance = this;
                DateTime = new DateTime(dateInMonth, season - 1, year, hour, minutes * 10);
            }
            else if (Instance != null && Instance != this)
            {
                Destroy(this);
            }

            DontDestroyOnLoad(Instance);
        }

        public void Start()
        {
            var gameState = GameStateLoader.Load(false);
            DateTime = gameState.CurrentDateTime;
            _sceneStates = gameState.SceneStates;
            
            OnDateTimeChanged?.Invoke(DateTime);
        }

        private void Update()
        {
            CurrentTimeBetweenTicks += Time.deltaTime;

            if (CurrentTimeBetweenTicks >= TimeBetweenTicks)
            {
                CurrentTimeBetweenTicks = 0;
                AdvanceTime();
            }
        }

        void AdvanceTime()
        {
            DateTime.AdvanceMinutes(TickMinutesIncreased);

            OnDateTimeChanged?.Invoke(DateTime); // Por qué se hace 2 veces?
        }

        public void AdvanceTime(int extraHours)
        {
            DateTime.AdvanceHours(extraHours);
            OnDateTimeChanged?.Invoke(DateTime);
        }
        
        private void OnEnable()
        {
            SceneManager.sceneUnloaded += UpdateSceneData;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= UpdateSceneData;
        }

        private void UpdateSceneData(Scene previous)
        {
            AddSceneLastTime(previous.name, DateTime);
        }

        
        public void AddSceneLastTime(string sceneName, DateTime time)
        {
            SceneStateData newData = new SceneStateData()
            {
                sceneName = sceneName,
                lastDateTime = time
            };
            
            int sceneIndex = _sceneStates.FindIndex(sceneData => sceneData.sceneName.Equals(sceneName));
        
            if (sceneIndex != -1) // If found data with Scene Name
            {
                _sceneStates[sceneIndex] = newData; // TODO: Capaz no crear data innecesariamente sino reemplazar los datos, y crear en el Else
            }
            else
            {
                _sceneStates.Add(newData);
            }
        }
    
        public SceneStateData GetSceneDataFromName(string sceneName)
        {
            return _sceneStates.FirstOrDefault(sceneData => sceneData.sceneName.Equals(sceneName));
        }

        public DateTime GetLastTimeInScene(string sceneName)
        {
            var time = GetSceneDataFromName(sceneName).lastDateTime;
            
            if (time.TotalNumDays == 0) return DateTime; // If there's no saved time, return current
            return time;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace FaRUtils.Systems.DateTime
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

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

        public static DateTime DateTime;

        [Header("Opciones de Tiempo")]

        public int TickMinutesIncreased = 10;
        public static float TimeBetweenTicks = 10f;
        private float CurrentTimeBetweenTicks = 0;

        public static LocalizedString localizedStringClock;

        public static UnityAction<DateTime> OnDateTimeChanged;

        public static UnityEvent<int> _OnHourChanged => DateTime.OnHourChanged;

        private void Awake()
        {
            DateTime = new DateTime(dateInMonth, season - 1, year, hour, minutes * 10);
            SetInstance();
        }

        private void SetInstance()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != null && Instance != this)
            {
                Destroy(this);
            }

            DontDestroyOnLoad(Instance);
        }

        public void Start()
        {
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

            OnDateTimeChanged?.Invoke(DateTime);
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
    }
}
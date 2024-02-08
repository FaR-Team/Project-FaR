using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace FaRUtils.Systems.DateTime
{
    public class TimeManager : MonoBehaviour
    {
        [Header("Opciones de Fecha y Tiempo")]
        [Range (1, 28), Header("Dia del Mes")]
        public int dateInMonth;
        [Range (1, 8), Header("Época del Año")]
        public int season;
        [Range (1, 99), Header("Año")]
        public int year;
        [Range (0, 24), Header("Hora")]
        public int hour;
        [Range (0, 6), Header("Minuto")]
        public int minutes;

        public DateTime DateTime;

        [Header("Opciones de Tiempo")]
        public int TickMinutesIncreased = 10;
        public float TimeBetweenTicks = 9f;
        private float CurrentTimeBetweenTicks = 0;

        public static LocalizedString localizedStringClock;

        public static UnityAction<DateTime> OnDateTimeChanged;

        private void Awake()
        {
            DateTime = new DateTime(dateInMonth, season - 1, year, hour, minutes * 10);
        }

        public void Start() 
        {
            OnDateTimeChanged?.Invoke(DateTime);

            //myLocalVariable = localizedString.GetVariable("myLocalVariable");
        }


        private void Update()
        {
            CurrentTimeBetweenTicks += Time.deltaTime;

            if (CurrentTimeBetweenTicks >= TimeBetweenTicks)
            {
                CurrentTimeBetweenTicks = 0;
                Tick();
            }

            OnDateTimeChanged?.Invoke(DateTime);
        }

        void Tick()
        {
            AdvanceTime();
        }

        void AdvanceTime()
        {
            DateTime.AdvanceMinutes(TickMinutesIncreased);

            OnDateTimeChanged?.Invoke(DateTime);
        }
    }

    [System.Serializable]
    public struct DateTime 
    {
        #region Campos
        private DaysEn day;
        private DaysEs dayEs;
        [SerializeField] private int date;
        [SerializeField] private int year;

        [SerializeField] private int hour;
        [SerializeField] private int minutes;

        [SerializeField] private Season season;


        private int totalNumDays;
        private int totalNumWeeks;
        #endregion

        #region Propiedades

        public DaysEn Day => day;
        public DaysEs DayEs => dayEs;
        public int Date => date;
        public int Hour => hour;
        public int Minutes => minutes;
        public Season Seasons => season;
        public int Year => year;
        public int TotalNumDays => totalNumDays;
        public int TotalNumWeeks => totalNumWeeks;
        public int CurrentWeek => totalNumWeeks % 16 == 0 ? 16 : totalNumWeeks % 16;

        #endregion

        #region Constructores
        public DateTime(int date, int season, int year, int hour, int minutes)
        {
            this.day = (DaysEn)(date % 7);
            if (day == 0) day = (DaysEn)7;

            this.dayEs = (DaysEs)(date % 7);
            if (dayEs == 0) dayEs = (DaysEs)7;

            this.date = date;
            this.season = (Season)season;
            this.year = year;

            this.hour = hour;
            this.minutes = minutes;

            totalNumDays = date + (28 * (int)this.season) + (112 * (year - 1));

            totalNumWeeks = 1 + totalNumDays / 7;
        }

        #endregion

        #region Avance del Tiempo
        public void AdvanceMinutes(int SecondsToAdvanceBy)
        {
            if (minutes + SecondsToAdvanceBy >= 60)
            {
                minutes = (minutes + SecondsToAdvanceBy) % 60;
                AdvanceHour();
            }
            else
            {
                minutes += SecondsToAdvanceBy;
            }
        }

        private void AdvanceHour()
        {
            if ((hour + 1) == 24)
            {
                hour = 0;
                AdvanceDay();
            }
            else
            {
                hour++;
            }
        }

        public void AdvanceDay()
        {
            if (day + 1 > (DaysEn)7)
            {
                day = (DaysEn)1;
                totalNumWeeks++;
            }
            else
            {
                day++;
            }

            if (dayEs + 1 > (DaysEs)7)
            {
                dayEs = (DaysEs)1;
                totalNumWeeks++;
            }
            else
            {
                dayEs++;
            }

            date++;

            if (date % 29 == 0)
            {
                AdvanceSeason();
                date = 1;
            }

            totalNumDays++;
        }

        private void AdvanceSeason()
        {
            if (season == Season.Late_Winter)
            {
                season = Season.Early_Spring;
                AdvanceYear();
            }
            else
            {
                season++;
            }
        }

        private void AdvanceYear()
        {
            date = 1;
            year++;
        }

        public void ContarDías(int Cantidad)
        {
            if(Date >= (Date + Cantidad))
            {
                Debug.Log("Creció UwU");
            }
        }

        #endregion

        #region Booleanos
        public bool IsNight()
        {
            return hour >= 18 || hour <= 6;
        }

        public bool IsMorning()
        {
            return hour >= 6 && hour < 12;
        }

        public bool IsAfternoon()
        {
            return hour >= 12 && hour < 18;
        }

        public bool IsWeekend()
        {
            return day > DaysEn.Fri ? true : false;
        }

        public bool IsParticularDay(DaysEn _day)
        {
            return day == _day;
        }
        #endregion

        #region Fechas Clave

        //Acá podemos añadir múltiples fechas especiales :p

        public DateTime NewYearsDay(int year)
        {
            if (year == 0) year = 1;
            {
                return new DateTime(1, 0, year, 6, 0);

                //Se ponen fechas así: Datetime(Día, Época, Año, Hora, Minuto)
            }
        }

        #endregion

        #region Inicio de Epoca

        public DateTime StartOfSeason()
        {
            return new DateTime(1, (int)season, year, 0, 0);
        }
        #endregion

        #region To Strings
        public override string ToString()
        {
            return $"Fecha: {DateToStringEs()} Época: {season} Hora: {TimeToString12()}" +
                $"\nTotal de Días: {totalNumDays} | Total de Semanas: {totalNumWeeks}";
        }

        public string DateToStringEn()
        {
            //UpdateDayLocals();
            return $"{day} {date}";
        }

        public string DateToStringEs()
        {
            //UpdateDayLocals();
            return $"{dayEs} {date}";
        }

        public string DateToStringSw()
        {
            //UpdateDayLocals();
            return $"Idks {date}";
        }

        public string TimeToString12()
        {
            int AdjustedHour = 0;
            if (hour == 0)
            {
                AdjustedHour = 12;
            }
            else if ( hour >= 13)
            {
                AdjustedHour = hour - 12;
            }
            else
            {
                AdjustedHour = hour;
            }

            string AmPm = hour > 12 ? "PM" : "AM";

            return $"{AdjustedHour.ToString("D2")}:{minutes.ToString("D2")} {AmPm}";
        }

        public string TimeToString24()
        {
            return $"{hour.ToString("D2")}:{minutes.ToString("D2")}";
        }

        #endregion

        [System.Serializable]
        public enum DaysEn
        {
        NULL = 0,
        Mon = 1,
        Tue = 2,
        Wed = 3,
        Thu = 4,
        Fri = 5,
        Sat = 6,
        Sun = 7
        }

        [System.Serializable]
        public enum DaysEs
        {
        NULL = 0,
        Lun = 1,
        Mar = 2,
        Mié = 3,
        Jue = 4,
        Vie = 5,
        Sab = 6,
        Dom = 7
        }

        [System.Serializable]
        public enum Season
        {
        Early_Spring = 0,
        Late_Spring = 1,
        Early_Summer = 2,
        Late_Summer = 3,
        Early_Fall = 4,
        Late_Fall = 5,
        Early_Winter = 6,
        Late_Winter = 7
        }
    }
}
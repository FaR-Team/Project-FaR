using UnityEngine;
using UnityEngine.Events;

namespace FaRUtils.Systems.DateTime
{
    [System.Serializable]
    public struct DateTime
    {

        private DaysId day;

        [SerializeField] private int date;
        [SerializeField] private int year;

        [SerializeField] private int hour;
        [SerializeField] private int minutes;

        [SerializeField] private Season season;


        private int totalNumDays;
        private int totalNumWeeks;



        public DaysId Day => day;
        public int Date => date;
        public int Hour => hour;
        public int Minutes => minutes;
        public Season Seasons => season;
        public int Year => year;
        public int TotalNumDays => totalNumDays;
        public int TotalNumWeeks => totalNumWeeks;
        public int CurrentWeek => totalNumWeeks % 16 == 0 ? 16 : totalNumWeeks % 16;

        public static UnityEvent<int> OnHourChanged;

        public DateTime(int date, int season, int year, int hour, int minutes)
        {
            day = (DaysId)(date % 7);
            if (day == 0) day = (DaysId)7;

            this.date = date;
            this.season = (Season)season;
            this.year = year;

            this.hour = hour;
            this.minutes = minutes;

            totalNumDays = date + (28 * (int)this.season) + (112 * (year - 1));

            totalNumWeeks = 1 + totalNumDays / 7;

            OnHourChanged = new UnityEvent<int>();
        }


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
        public void AdvanceHours(int hoursToAdvance)
        {
            var newHours = hour + hoursToAdvance;

            if (newHours == 24)
            {
                hour = 0;
                OnHourChanged.Invoke(hour);
                AdvanceDay();
            }
            else
            {
                hour = newHours;
                OnHourChanged.Invoke(hour);
            }
        }

        private void AdvanceHour()
        {
            if ((hour + 1) == 24)
            {
                hour = 0;
                OnHourChanged.Invoke(hour);
                AdvanceDay();
            }
            else
            {
                hour++;
                OnHourChanged.Invoke(hour);
            }
        }


        public void AdvanceDay()
        {
            if (day + 1 > DaysId.SunId)
            {
                day = DaysId.MonId;
                totalNumWeeks++;
            }
            else
            {
                day++;
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


        public readonly bool IsNight()
        {
            return hour >= 18 || hour <= 6;
        }

        public readonly bool IsMorning()
        {
            return hour >= 6 && hour < 12;
        }

        public readonly bool IsAfternoon()
        {
            return hour >= 12 && hour < 18;
        }

        public readonly bool IsWeekend()
        {
            return day > DaysId.FriId;
        }

        public readonly bool IsParticularDay(DaysId _day)
        {
            return day == _day;
        }

        public readonly string TimeToString12()
        {
            int AdjustedHour;

            if (hour == 0)
            {
                AdjustedHour = 12;
            }
            else if (hour >= 13)
            {
                AdjustedHour = hour - 12;
            }
            else
            {
                AdjustedHour = hour;
            }

            string AmPm = hour > 12 ? "PM" : "AM";

            return $"{AdjustedHour:D2}:{minutes:D2} {AmPm}";
        }

        public string TimeToString24()
        {
            return $"{hour:D2}:{minutes:D2}";
        }

        [System.Serializable]
        public enum DaysId
        {
            Null, MonId, TueId, WedId, ThuId, FriId, SatId, SunId
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
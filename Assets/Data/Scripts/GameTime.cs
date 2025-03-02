using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameTime : MonoBehaviour
{
    private int[] monthLenght = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    private string[] monthAbb = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

    public static event EventHandler<OnNewHourEvent> OnNewHour;
    public static event EventHandler OnNewDay;
    public static event EventHandler OnNewWeek;
    public static event EventHandler OnNewMonth;
    public static event EventHandler OnNewYear;

    private TextMeshProUGUI textMesh;

    public static int weekDay, monthDay, month, hour, year;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        weekDay = 2;
        hour = 0;
        monthDay = 0;
        year = 1936;
        month = 0;
        
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void NextHour(out TimeData time)
    {
        hour++;
        if (hour >= 24)
        {
            hour = 0;
            weekDay++;
            monthDay++;
            //OnNewDay.Invoke(this, null);

            if (weekDay >= 7)
            {
                weekDay = 0;
                //OnNewWeek.Invoke(this, null);
            }

            if (monthDay >= monthLenght[month])
            {
                monthDay = 0;
                month++;
                //OnNewMonth.Invoke(this, null);
                if (month >= 12)
                {
                    month = 0;
                    //OnNewYear.Invoke(this, null);
                }
            }
        }

        time = new TimeData()
        {
            weekDay = weekDay,
            monthDay = monthDay,
            month = month,
            hour = hour,
            year = year
        };

        OnNewHour?.Invoke(this, new OnNewHourEvent
        {
            hour = hour,
        });
    }

    public string GetTimeStamp()
    {
        return hour + ":00, " + (monthDay + 1) + ". " + monthAbb[month] + " " + year;
    }

    public void ReceiveTimeUpdate(TimeData time)
    {
        weekDay = time.weekDay;
        monthDay = time.monthDay;
        month = time.month;
        hour = time.hour;
        year = time.year;

        UpdateTextMesh();
    }

    private void UpdateTextMesh()
    {
        if (textMesh != null)
        {
            textMesh.text = GetTimeStamp();
        }
    }



    /*
     * Events concerning different Time Updates
     */

    public class OnNewHourEvent : EventArgs
    {
        public int hour;
    }

    public class NewWeekEvent : EventArgs
    {}

    public class NewDayEvent : EventArgs
    {
        /*private int monthDay;
        private int weekDay;

        public NewDayEvent(int monthDay, int weekDay)
        {
            this.weekDay = weekDay;
            this.monthDay = monthDay;
        }

        public int GetMonthDay()
        {
            return monthDay;
        }

        public int GetWeekDay()
        {
            return weekDay;
        }*/
    }

    public class NewMonthEvent : EventArgs
    { }
    
    
    public class NewYearEvent : EventArgs
    { }

    public class TimeData
    {
        public int weekDay, monthDay, month, hour, year;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParentalControls.Duex.Library
{
    public struct Time
    {
        public int Hour;
        public int Minute;
        public int Second;

        public Time(int hour, int minute, int second)
        {
            Hour = MathF.Clamp(hour, 1, 12);
            Minute = MathF.Clamp(minute, 1, 60);
            Second = MathF.Clamp(second, 1, 60);
        }

        public Time(int hour, int minute)
        {
            Hour = MathF.Clamp(hour, 1, 12);
            Minute = MathF.Clamp(minute, 1, 60);
            Second = 0;
        }

        public Time(int hour)
        {
            Hour = MathF.Clamp(hour, 1, 12);
            Minute = 0;
            Second = 0;
        }

        public static Time operator +(Time timea, Time timeb)
        {
            return new Time(timea.Hour + timeb.Hour, timea.Minute + timeb.Minute, timea.Second + timeb.Second);
        }

        public static Time operator -(Time timea, Time timeb)
        {
            return new Time(timea.Hour - timeb.Hour, timea.Minute - timeb.Minute, timea.Second - timeb.Second);
        }

        public static Time operator *(Time timea, Time timeb)
        {
            return new Time(timea.Hour * timeb.Hour, timea.Minute * timeb.Minute, timea.Second * timeb.Second);
        }

        public static Time operator /(Time timea, Time timeb)
        {
            return new Time(timea.Hour / timeb.Hour, timea.Minute / timeb.Minute, timea.Second / timeb.Second);
        }

        public static Time operator +(Time timea, int timeb)
        {
            return new Time(timea.Hour + timeb, timea.Minute + timeb, timea.Second + timeb);
        }

        public static Time operator -(Time timea, int timeb)
        {
            return new Time(timea.Hour - timeb, timea.Minute - timeb, timea.Second - timeb);
        }

        public static Time operator *(Time timea, int timeb)
        {
            return new Time(timea.Hour * timeb, timea.Minute * timeb, timea.Second * timeb);
        }

        public static Time operator /(Time timea, int timeb)
        {
            return new Time(timea.Hour / timeb, timea.Minute / timeb, timea.Second / timeb);
        }

        public static bool operator ==(Time timea, object obj)
        {
            return timea.Equals(obj);
        }

        public static bool operator !=(Time timea, object obj)
        {
            return !timea.Equals(obj);
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType() != typeof(Time))
                return false;
            Time timeb = (Time)obj;
            return (this.Hour == timeb.Hour) && (this.Minute == timeb.Minute) && (this.Second == timeb.Second);
        }

    }

    public class Reminder
    {

        public Time Time;

        public DayOfWeek Days;

        public TimeSpan Duration;

        public string Name;

        public IReminderAction[] Actions;

    }
}

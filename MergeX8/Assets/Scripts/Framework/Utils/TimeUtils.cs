using System;
using DragonPlus;
using DragonU3DSDK.Network.API;

namespace Framework
{
    public class TimeUtils
    {
        public static ulong GetTimeStampMilliseconds()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            ulong timeStamp = (ulong) (DateTime.Now - startTime).TotalMilliseconds;
            return timeStamp;
        }

        public static ulong GetTimeStamp()
        {
            var ms = GetTimeStampMilliseconds();
            return ms / 1000;
        }

        public static DateTime GetDateTime(long timeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            long unixTimeStampInTicks = timeStamp * TimeSpan.TicksPerMillisecond;
            return new DateTime(dateTime.Ticks + unixTimeStampInTicks, DateTimeKind.Utc);
        }

        public static string GetDateTimeString(DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToString("yyyy-MM-dd  HH:mm:ss");
        }

        public static string GetDateByTimeStamp(long timeStamp)
        {
            return GetDateTimeString(GetDateTime(timeStamp));
        }

        public static string GetTimeMinutesString(int second)
        {
            int leftMinutes = second / 60;
            int leftSeconds = second % 60;

            return $"{leftMinutes:d2}:{leftSeconds:d2}";
        }

        public static string GetTimeString(int second, bool shortStr)
        {
            int hour = (int) second / 3600;
            int leftMinutes = (second % 3600) / 60;
            int leftSeconds = second % 60;
            if (hour == 0)
            {
                if (shortStr)
                {
                    return $"{leftMinutes}m";
                }
                else
                {
                    return $"{leftMinutes:d2}:{leftSeconds:d2}";
                }
            }

            if (hour >= 24)
            {
                if (shortStr)
                {
                    return $"{hour / 24}d";
                }
                else
                {
                    return $"{hour / 24:d2}:{hour % 24:d2}";
                }
            }

            if (shortStr)
            {
                return $"{hour}h";
            }
            else
            {
                return $"{hour:d2}:{leftMinutes:d2}";
            }
        }

        public static string GetTimeString(int second)
        {
            int hour = (int) second / 3600;
            int leftMinutes = (second % 3600) / 60;
            int leftSeconds = second % 60;
            string dstr = LocalizationManager.Instance.GetLocalizedString("UI_common_time_d");
            string hstr = LocalizationManager.Instance.GetLocalizedString("UI_common_time_h");
            string mstr = LocalizationManager.Instance.GetLocalizedString("UI_common_time_m");
            string sstr = LocalizationManager.Instance.GetLocalizedString("UI_common_time_s");
            string timeString = "";
            if (hour == 0)
            {
                return FormatString(leftMinutes, leftSeconds, mstr, sstr);
            }

            if (hour >= 24)
            {
                int h = hour / 24;
                int m = hour % 24;

                return FormatString(h, m, dstr, hstr);
            }

            return FormatString(hour, leftMinutes, hstr, mstr);
        }

        private static string FormatString(int num1, int num2, string f1, string f2)
        {
            string formatString = "";

            if (num1 >= 10)
                formatString = $"{num1:d2}" + f1;
            else if (num1 > 0)
                formatString = $"{num1}" + f1;

            if (num2 >= 10)
                formatString += $"{num2:d2}" + f2;
            else if (num2 > 0)
                formatString += $"{num2}" + f2;

            return formatString;
        }
    }
}
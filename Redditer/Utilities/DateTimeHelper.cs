using System;

namespace Redditer.Utilities
{
    public static class DateTimeHelper
    {
        private const ulong SecondsInMinute = 60;
        private const ulong MinutesInHour = 60;
        private const ulong HoursInDay = 24;
        private const ulong DaysInMonth = 30;
        private const ulong MonthsInYear = 12;

        private const ulong SecondsInHour = SecondsInMinute * MinutesInHour;
        private const ulong SecondsInDay = HoursInDay * SecondsInHour;
        private const ulong SecondsInMonth = DaysInMonth * SecondsInDay;
        private const ulong SecondsInYear = MonthsInYear * SecondsInMonth;

        public static DateTime FromTimestamp(ulong timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
        }

        public static string RelativeFromNow(this DateTime dateTime)
        {
            var diff = DateTime.UtcNow - dateTime;
            var secs = (ulong)diff.Seconds + SecondsInMinute * (ulong)diff.Minutes + SecondsInHour * (ulong)diff.Hours + SecondsInDay * (ulong)diff.Days;

            if (secs == 0)
                return "less than second ago";

            if (secs < SecondsInMinute)
                return "less than minute ago";

            if (secs < SecondsInHour)
            {
                var mins = secs/SecondsInMinute;
                return string.Format(mins == 1 ? "{0} minute ago" : "{0} minutes ago", mins);
            }

            if (secs < SecondsInDay)
            {
                var hrs = secs/SecondsInHour;
                return string.Format(hrs == 1 ? "{0} hour ago" : "{0} hours ago", hrs);
            }

            if (secs < SecondsInMonth)
            {
                var days = secs/SecondsInDay;
                return string.Format(days == 1 ? "{0} day ago" : "{0} days ago", days);
            }

            if (secs < SecondsInYear)
            {
                var months = secs/SecondsInMonth;
                return string.Format(months == 1 ? "{0} month ago" : "{0} months ago", months);
            }

            var years = secs/SecondsInYear;
            return string.Format(years == 1 ? "{0} year ago" : "{0} years ago", years);
        }
    }
}

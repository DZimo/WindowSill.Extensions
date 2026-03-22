using Microsoft.Graph.Models;

namespace WindowSill.OutlookCalendar.Extensions
{
    public static class AdditionnalExtensions
    {
        public static DateTime ToDateTimeExt(this DateTimeTimeZone dateTimeZone)
        {
            var res = DateTime.Parse(dateTimeZone.DateTime ?? DateTime.Now.ToString());
            return res;
        }

        public static DateTimeTimeZone ToDateTimeZoneExt(this DateTime dateTime)
        {
            return new DateTimeTimeZone
            {
                DateTime = dateTime.ToString("o"),
                TimeZone = TimeZoneInfo.Local.Id
            };
        }
    }
}

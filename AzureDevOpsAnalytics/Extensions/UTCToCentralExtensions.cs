using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDevOpsAnalytics.Extensions
{
    public static class UTCToCentralExtensions
    {
        public static DateTime ToCentralTimeZone(this DateTime dateTime)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }
    }
}

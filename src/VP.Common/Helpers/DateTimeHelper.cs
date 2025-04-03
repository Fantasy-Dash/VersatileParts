using System.Globalization;

namespace VP.Common.Helpers
{
    public class DateTimeHelper
    {
        public static string DateTimeToDMTFDateTime(DateTime dateTime)
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Utc => $"{dateTime:yyyyMMddHHmmss}.{dateTime.Microsecond,6}+{TimeZoneInfo.Utc.BaseUtcOffset.TotalMinutes,3}",
                _ => $"{dateTime:yyyyMMddHHmmss}.{dateTime.Microsecond,6}+{TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes,3}",
            };
        }

        public static DateTime DMTFToUTCDateTime(string dmtfDateTime)
        {
            DateTime dt = DateTime.ParseExact(dmtfDateTime[..14], "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                .AddMicroseconds(Convert.ToInt32(dmtfDateTime.Substring(15, 6)))
                -TimeSpan.FromMinutes(int.Parse(dmtfDateTime[21..]));
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            return dt;
        }
    }
}

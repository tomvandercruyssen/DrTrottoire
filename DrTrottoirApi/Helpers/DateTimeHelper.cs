namespace DrTrottoirApi.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime GetBelgianTime()
        {
            var belgianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, belgianTimeZone);
        }
    }
}

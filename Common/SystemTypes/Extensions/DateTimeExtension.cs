namespace Common.SystemTypes.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime NextDayOfWeek(this DateTime from, DayOfWeek dayOfWeek)
        {
            return from.AddDays(((int)dayOfWeek - (int)from.DayOfWeek + 7) % 7);
        }
    }
}

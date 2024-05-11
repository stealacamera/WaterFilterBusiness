namespace WaterFilterBusiness.Common.Utilities;

public static class DateTimeUtility
{
    public static DateTime StartOfWeek(this DateTime datetime, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        int diff = (7 + (datetime.DayOfWeek - startOfWeek)) % 7;
        return datetime.AddDays(-1 * diff).Date;
    }
    public static DateTime EndOfWeek(this DateTime datetime, DayOfWeek endOfWeek = DayOfWeek.Sunday)
    {
        if (datetime.DayOfWeek == endOfWeek)
            return datetime.Date.Date.AddDays(1).AddMilliseconds(-1);
        else
        {
            var diff = datetime.DayOfWeek - endOfWeek;
            return datetime.AddDays(7 - diff).Date.AddDays(1).AddMilliseconds(-1);
        }
    }
}

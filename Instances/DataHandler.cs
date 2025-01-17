using System.Globalization;
using ZarzadzanieFinansami;

namespace ZarządzanieFinansami.Instances;

public class DateHandler
{
    public readonly DateTime FirstDayOfWeek;
    public readonly DateTime LastDayOfWeek;
    public readonly DateTime FirstDayOfMonth;
    public readonly DateTime LastDayOfMonth;
    public readonly DateTime FirstDayOfYear;
    public readonly DateTime LastDayOfYear;

    public DateHandler()
    {
        DateTime today = DateTime.Today;
        
        DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        int diffToFirstDayOfWeek = (7 + (today.DayOfWeek - firstDayOfWeek)) % 7;

        FirstDayOfWeek = today.AddDays(-diffToFirstDayOfWeek);
        LastDayOfWeek = FirstDayOfWeek.AddDays(6);
        
        FirstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        LastDayOfMonth = FirstDayOfMonth.AddMonths(1).AddDays(-1);

        FirstDayOfYear = new DateTime(today.Year, 1, 1);
        LastDayOfYear = new DateTime(today.Year, 12, 31);
    }

    public void PrintDates()
    {
        Console.WriteLine($"First day of the week: {FirstDayOfWeek:yyyy-MM-dd}");
        Console.WriteLine($"Last day of the week: {LastDayOfWeek:yyyy-MM-dd}");
        Console.WriteLine($"First day of the month: {FirstDayOfMonth:yyyy-MM-dd}");
        Console.WriteLine($"Last day of the month: {LastDayOfMonth:yyyy-MM-dd}");
        Console.WriteLine($"First day of the year: {FirstDayOfYear:yyyy-MM-dd}");
        Console.WriteLine($"Last day of the year: {LastDayOfYear:yyyy-MM-dd}");
    }

    public DateTime GetStartDateFromRange(DataRange range)
    {
        return range switch
        {
            DataRange.Year => FirstDayOfYear,
            DataRange.Month => FirstDayOfMonth,
            DataRange.Week => FirstDayOfWeek,
            _ => throw new ArgumentOutOfRangeException(nameof(range), range, null)
        };
    }
    public DateTime GetLastDateFromRange(DataRange range)
    {
        return range switch
        {
            DataRange.Year => LastDayOfYear,
            DataRange.Month => LastDayOfMonth,
            DataRange.Week => LastDayOfWeek,
            _ => throw new ArgumentOutOfRangeException(nameof(range), range, null)
        };
    }
    public static void GetDatesFromRange(out DateTime startDate, out DateTime lastDate)
    {
        DateHandler dh = new DateHandler();
        startDate = dh.GetStartDateFromRange(Core.GlobalDataRange);
        lastDate = dh.GetLastDateFromRange(Core.GlobalDataRange);
    }
    public static void GetDatesFromRange(DataRange range, out DateTime startDate, out DateTime lastDate)
    {
        DateHandler dh = new DateHandler();
        startDate = dh.GetStartDateFromRange(range);
        lastDate = dh.GetLastDateFromRange(range);
    }
}
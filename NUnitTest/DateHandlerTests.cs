using ZarządzanieFinansami.Instances;

namespace ZarzadzanieFinansami.Tests;

[TestFixture]
public class DateHandlerTests
{
    private DateHandler _dateHandler;

    [SetUp]
    public void Setup()
    {
        _dateHandler = new DateHandler();
    }

    [Test]
    public void GetDatesFromRange_ForYearRange_ReturnsFirstAndLastDayOfYear()
    {
        DateHandler.GetDatesFromRange(DataRange.Year, out DateTime startDate, out DateTime lastDate);
        Assert.That(startDate, Is.EqualTo(_dateHandler.FirstDayOfYear));
        Assert.That(lastDate, Is.EqualTo(_dateHandler.LastDayOfYear));
    }

    [Test]
    public void GetDatesFromRange_ForMonthRange_ReturnsFirstAndLastDayOfMonth()
    {
        DateHandler.GetDatesFromRange(DataRange.Month, out DateTime startDate, out DateTime lastDate);
        Assert.That(startDate, Is.EqualTo(_dateHandler.FirstDayOfMonth));
        Assert.That(lastDate, Is.EqualTo(_dateHandler.LastDayOfMonth));
    }

    [Test]
    public void GetDatesFromRange_ForWeekRange_ReturnsFirstAndLastDayOfWeek()
    {
        DateHandler.GetDatesFromRange(DataRange.Week, out DateTime startDate, out DateTime lastDate);
        Assert.That(startDate, Is.EqualTo(_dateHandler.FirstDayOfWeek));
        Assert.That(lastDate, Is.EqualTo(_dateHandler.LastDayOfWeek));
    }

    [Test]
    public void GetDatesFromRange_WithInvalidDataRange_ThrowsArgumentOutOfRangeException()
    {
        DataRange invalidRange = (DataRange)999;
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            DateHandler.GetDatesFromRange(invalidRange, out _, out _);
        });
        Assert.That(ex.ParamName, Is.EqualTo("range"));
    }
}
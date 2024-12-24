using NUnit.Framework;
using ZarzadzanieFinansami;

namespace ZarzadzanieFinansami.Tests;

using NUnit.Framework;

[TestFixture]
public class TransactionTests
{
    private Transaction transaction1;
    private Transaction transaction2;

    [SetUp]
    public void SetUp()
    {
        transaction1 = new Transaction(1, "Test1", 200.0, "10.01.2023", "kot");
        transaction2 = new Transaction(2, "Test2", 300.0, "01.12.2024", "Pies");
    }

    [Test]
    public void CompareTo_ShouldThrowArgumentException_WhenInvalidParameter()
    {
        Assert.Throws<ArgumentException>(() => transaction1.CompareTo(null));
    }
    
    [Test]
    public void CompareTo_ShouldThrowArgumentException_WhenNoParameter()
    {
        Assert.Throws<ArgumentException>(() => transaction1.CompareTo(transaction2));
    }

    [Test]
    public void CompareTo_ShouldCompareByID()
    {
        int result = transaction1.CompareTo(transaction2, ComparisonField.ID);
        Assert.That(result, Is.LessThan(0));
    }

    [Test]
    public void CompareTo_ShouldCompareByName()
    {
        int result = transaction1.CompareTo(transaction2, ComparisonField.Nazwa);
        Assert.That(result, Is.LessThan(0));
    }

    [Test]
    public void CompareTo_ShouldCompareByAmount()
    {
        int result = transaction1.CompareTo(transaction2, ComparisonField.Kwota);
        Assert.That(result, Is.LessThan(0));
    }

    [Test]
    public void CompareTo_ShouldCompareByDate()
    {
        int result = transaction1.CompareTo(transaction2, ComparisonField.Data);
        Assert.That(result, Is.LessThan(0));
    }

    [Test]
    public void CompareTo_ShouldCompareByNotes()
    {
        int result = transaction1.CompareTo(transaction2, ComparisonField.Uwagi);
        Assert.That(result, Is.LessThan(0));
    }
}
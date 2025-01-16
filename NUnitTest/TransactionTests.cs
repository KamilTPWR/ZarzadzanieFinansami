using NUnit.Framework;
using ZarzadzanieFinansami;

namespace ZarzadzanieFinansami.Tests;

[TestFixture]
public class TransactionTests
{
    private Transaction _transaction1;
    private Transaction _transaction2;

    [SetUp]
    public void SetUp()
    {
        _transaction1 = new Transaction(1, "Test1", 200.0, "2023-10-01", "kots", "kat1");
        _transaction2 = new Transaction(2, "Test2", 300.0, "2024-12-01", "Pies","kat1");
    }

    [Test]
    public void CompareTo_ShouldThrowArgumentException_WhenInvalidParameter()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var i = _transaction1.CompareTo(null);
        });
    }
    
    [Test]
    public void CompareTo_ShouldThrowArgumentException_WhenNoParameter()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var i = _transaction1.CompareTo(_transaction2);
        });
    }

    [Test]
    public void CompareTo_ShouldCompareByID()
    {
        int result = _transaction1.CompareTo(_transaction2, ComparisonField.ID);
        Assert.That(result, Is.LessThan(0));
    }

    [Test]
    public void CompareTo_ShouldCompareByName()
    {
        int result = _transaction1.CompareTo(_transaction2, ComparisonField.Nazwa);
        Assert.That(result, Is.LessThan(0));
    }

    [Test]
    public void CompareTo_ShouldCompareByAmount()
    {
        int result = _transaction1.CompareTo(_transaction2, ComparisonField.Kwota);
        Assert.That(result, Is.LessThan(0));
    }

    [Test]
    public void CompareTo_ShouldCompareByDate()
    {
        int result = _transaction1.CompareTo(_transaction2, ComparisonField.Data);
        Assert.That(result, Is.LessThan(0));
    }

    [Test]
    public void CompareTo_ShouldCompareByNotes()
    {
        int result = _transaction1.CompareTo(_transaction2, ComparisonField.Uwagi);
        Assert.That(result, Is.LessThan(0));
    }
}
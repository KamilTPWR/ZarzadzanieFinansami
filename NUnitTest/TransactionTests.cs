using NUnit.Framework;
using ZarzadzanieFinansami;

[TestFixture]
public class TransactionTests
{

    [Test]
    public void CompareTo_ShouldReturnNegative_WhenNull()
    {
        var transaction1 = new Transaction(1, "Test1", 200.0, " ", " ");
        
        int result = transaction1.CompareTo(null);
        
        Assert.That(result, Is.EqualTo(-1));
    }

    [Test]
    public void CompareTo_ShouldCompareKwotaCorrectly()
    {
          
        var transaction1 = new Transaction(1, "Test1", 200.0, " ", " ");
        var transaction2 = new Transaction(2, "Test2", 300.0, " ", " ");

         
        int result = transaction1.CompareTo(transaction2);

           
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void CompareTo_ShouldReturnZero_WhenKwotaIsEqual()
    {
          
        var transaction1 = new Transaction(1, "Test1", 200.0, " ", " ");
        var transaction2 = new Transaction(2, "Test2", 200.0, " ", " ");

         
        int result = transaction1.CompareTo(transaction2);

           
        Assert.That(result, Is.EqualTo(0)); 
    }
}

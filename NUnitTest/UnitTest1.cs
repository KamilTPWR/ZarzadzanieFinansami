using System.Diagnostics;
using ZarzadzanieFinansami;

namespace NUnitTest;

public class Tests
{
   [Test]
    public void Test1()
    {
        var testBool  = StrUtillity.IsNumberFormatValid("0123456789");
        Assert.IsTrue(testBool);
    }
    [Test] 
    public void Test2()
    {
        var testBool  = StrUtillity.IsNumberFormatValid("0123456789");
        Assert.IsFalse(!testBool);
    }
}
 
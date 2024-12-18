using NUnit.Framework;
using ZarzadzanieFinansami;


[TestFixture]
public class StrUtilityTests
{
    [Test]
    public void IsNumberFormatValid_ShouldReturnTrue_WhenInputIsValid()
    {
        string input = "123,456";
        
        bool result = StrUtillity.IsNumberFormatValid(input);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsNumberFormatValid_ShouldReturnFalse_WhenInputIsNotValid()
    {
        string input = "123a456";
        
        bool result = StrUtillity.IsNumberFormatValid(input);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void CropString_ShouldReturnOriginalString_WhenStringIsShorterThanOrEqualToSize()
    {
        string input = "Pięć";
        int size = 10;

        string result = StrUtillity.CropString(input, size);
        
        Assert.That(result, Is.EqualTo("Pięć"));
    }

    [Test]
    public void CropString_ShouldReturnCroppedString_WhenStringExceedsSize()
    {
        string input = "qwertyuiop";
        int size = 5;
        
        string result = StrUtillity.CropString(input, size);
        
        Assert.That(result, Is.EqualTo("qwert"));
    }

    [Test]
    public void NumberOfDigitsAfterComa_ShouldReturnZero_WhenNoCommaPresent()
    {
        // Arrange
        string input = "123456";

        // Act
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void NumberOfDigitsAfterComa_ShouldReturnCorrectCount_WhenCommaPresent()
    {
        // Arrange
        string input = "123,456";

        // Act
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

        // Assert
        Assert.AreEqual(3, result);
    }
    
    [Test]
    public void NumberOfDigitsAfterComa_ShouldReturnZero_WhenDotIsInTheString()
    {
        // Arrange
        string input = "123.";

        // Act
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void NumberOfDigitsAfterComa_ShouldReturnZero_WhenCommaIsAtTheEnd()
    {
        // Arrange
        string input = "123,";

        // Act
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }
    
    [Test]
    public void NumberOfDigitsAfterComa_ShouldReturnMinusOne_WhenMultipleCommasPresent()
    {
        // Arrange
        string input = "123,,123";

        // Act
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

        // Assert
        Assert.That(result, Is.EqualTo(-1));
    }
    
    [Test]
    public void NumberOfDigitsAfterComa_ShouldReturnMinusOne_WhenOnlyMultipleCommasPresent()
    {
        // Arrange
        string input = ",,";

        // Act
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

        // Assert
        Assert.That(result, Is.EqualTo(-1));
    }
    
    [Test]
    public void NumberOfDigitsAfterComa_ShouldReturnMinusOne_WhenStringIsEmpty()
    {
        // Arrange
        string input = String.Empty;

        // Act
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }
}
﻿using NUnit.Framework;
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
          
        string input = "123456";

         
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

           
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void NumberOfDigitsAfterComa_ShouldReturnCorrectCount_WhenCommaPresent()
    {
          
        string input = "123,456";

         
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

           
        Assert.AreEqual(3, result);
    }
    
    [Test]
    public void NumberOfDigitsAfterComa_ShouldReturnZero_WhenDotIsInTheString()
    {
          
        string input = "123.";

         
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

           
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void NumberOfDigitsAfterComa_ShouldReturnZero_WhenCommaIsAtTheEnd()
    {
          
        string input = "123,";

         
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

           
        Assert.That(result, Is.EqualTo(0));
    }
    
    [Test]
    public void NumberOfDigitsAfterComa_ShouldReturnMinusOne_WhenMultipleCommasPresent()
    {
          
        string input = "123,,123";

         
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

           
        Assert.That(result, Is.EqualTo(-1));
    }
    
    [Test]
    public void NumberOfDigitsAfterComa_ShouldReturnMinusOne_WhenOnlyMultipleCommasPresent()
    {
          
        string input = ",,";

         
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

           
        Assert.That(result, Is.EqualTo(-1));
    }
    
    [Test]
    public void NumberOfDigitsAfterComa_ShouldReturnMinusOne_WhenStringIsEmpty()
    {
          
        string input = String.Empty;

         
        int result = StrUtillity.NumberOfDigitsAfterComa(input);

           
        Assert.That(result, Is.EqualTo(0));
    }
}
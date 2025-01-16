namespace ZarzadzanieFinansami.Tests;

[TestFixture]
public class SettingsUtilityTests
{
    private string _testFilePath;
    private string expectedSaldo = "1000.50";
    private DataRange expectedDataRange = DataRange.Month;
    private Currency expectedCurrency = Currency.USD;
    private int expectedRows = 10;

    [SetUp]
    public void SetUp()
    {
        _testFilePath = Path.Combine(Path.GetTempPath(), "test_settings.ini");
        SettingsUtility.SaveSettings(expectedSaldo, expectedDataRange, expectedCurrency, expectedRows, _testFilePath);
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [Test]
    public void SaveSettings_ShouldCreateFileWithCorrectContent()
    {
        SettingsUtility.SaveSettings(expectedSaldo, expectedDataRange, expectedCurrency, expectedRows, _testFilePath);
        
        Assert.IsTrue(File.Exists(_testFilePath), "Settings file was not created.");
        
        string[] lines = File.ReadAllLines(_testFilePath);
        Assert.That(lines[0], Is.EqualTo("[Finance]"));
        Assert.That(lines[1], Is.EqualTo($"Saldo={expectedSaldo}"));
        Assert.That(lines[2], Is.EqualTo($"DataRange={expectedDataRange}"));
        Assert.That(lines[3], Is.EqualTo("[Display]"));
        Assert.That(lines[4], Is.EqualTo($"CurrencyType={expectedCurrency}"));
        Assert.That(lines[5], Is.EqualTo($"RowsToDisplay={expectedRows}"));
    }

    [Test]
    public void LoadSettings_ShouldReturnCorrectData()
    {
        using (var consoleOutput = new StringWriter())
        {
            Console.SetOut(consoleOutput);
            
            SettingsUtility.DebugLoadSettings(_testFilePath);
            
            string output = consoleOutput.ToString();
            StringAssert.Contains($"Saldo: {expectedSaldo}", output);
            StringAssert.Contains($"Data Range: {expectedDataRange}", output);
            StringAssert.Contains($"Currency Type: {expectedCurrency}", output);
            StringAssert.Contains($"Rows to Display: {expectedRows}", output);
        }
    }
    [Test]
    public void GetCurrencyType_ShouldReturnCorrectCurrency_WhenValidCurrencyIsProvided()
    {
        Currency result = SettingsUtility.GetCurrencyType(_testFilePath);
        Assert.That(result, Is.EqualTo(expectedCurrency));
    }

    [Test]
    public void GetDataRange_ShouldReturnCorrectDataRange_WhenValidDataRangeIsProvided()
    {
        DataRange result = SettingsUtility.GetDataRange(_testFilePath);
        Assert.That(result, Is.EqualTo(expectedDataRange));
    }

    [Test]
    public void GetRowsToDisplay_ShouldReturnCorrectNumberOfRows_WhenValidRowsToDisplayIsProvided()
    {
        int result = SettingsUtility.GetRowsToDisplay(_testFilePath);
        Assert.That(result, Is.EqualTo(expectedRows));
    }

    [Test]
    public void GetSaldo_ShouldReturnCorrectSaldo_WhenValidSaldoIsProvided()
    {
        string result = SettingsUtility.GetSaldo(_testFilePath);
        Assert.That(result, Is.EqualTo(expectedSaldo));
    }
}
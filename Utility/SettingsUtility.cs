using System.IO;

namespace ZarzadzanieFinansami;

public class SettingsUtility
{
    public static void SaveSettings(string saldoReadyToBeSaved, Currency currencyType, int rowsToDisplay,
        string colorSchema, string filePath = "settings.ini")
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("[Finance]");
            writer.WriteLine($"Saldo={saldoReadyToBeSaved}");
            writer.WriteLine("[Display]");
            writer.WriteLine($"CurrencyType={currencyType.ToString()}");
            writer.WriteLine($"RowsToDisplay={rowsToDisplay}");
            writer.WriteLine();
            writer.WriteLine("[Appearance]");
            writer.WriteLine($"ColorSchema={colorSchema}");
        }
    }

    public static Currency GetCurrencyType(string filePath = "settings.ini", Currency defaultCurrency = Currency.PLN)
    {
        if (!IsFileExist(filePath)) return defaultCurrency;
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string s = line.Substring("CurrencyType=".Length).Trim();
            if (Enum.TryParse(s, out Currency currencyType))
                return currencyType;
        }
        return defaultCurrency;
    }

    public static void LoadSettings(string filePath = "settings.ini")
    {
        if (!IsFileExist(filePath)) return;
        string[] lines = File.ReadAllLines(filePath);
        string saldo = string.Empty;
        string currencyType = string.Empty;
        string rowsToDisplay = string.Empty;
        string colorSchema = string.Empty;

        foreach (string line in lines)
        {
            if (line.StartsWith("Saldo="))
                saldo = line.Substring("Saldo=".Length).Trim();
            else if (line.StartsWith("CurrencyType="))
                currencyType = line.Substring("CurrencyType=".Length).Trim();
            else if (line.StartsWith("RowsToDisplay="))
                rowsToDisplay = line.Substring("RowsToDisplay=".Length).Trim();
            else if (line.StartsWith("ColorSchema="))
                colorSchema = line.Substring("ColorSchema=".Length).Trim();
        }

        Console.WriteLine("Loaded Settings:");
        Console.WriteLine($"Saldo: {saldo}");
        Console.WriteLine($"Currency Type: {currencyType}");
        Console.WriteLine($"Rows to Display: {rowsToDisplay}");
        Console.WriteLine($"Color Schema: {colorSchema}");
    }

    private static bool IsFileExist(string filePath)
    {
        if (File.Exists(filePath)) return true;
        Console.WriteLine("Settings file not found.");
        return false;
    }
}
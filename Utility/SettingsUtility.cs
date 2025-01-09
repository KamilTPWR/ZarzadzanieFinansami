using System.IO;

namespace ZarzadzanieFinansami;

public class SettingsUtility
{
    public static void SaveSettings(string saldoReadyToBeSaved, Currency currencyType, int rowsToDisplay, string filePath = "settings.ini")
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("[Finance]");
            writer.WriteLine($"Saldo={saldoReadyToBeSaved}");
            writer.WriteLine("[Display]");
            writer.WriteLine($"CurrencyType={currencyType.ToString()}");
            writer.WriteLine($"RowsToDisplay={rowsToDisplay}");
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

        foreach (string line in lines)
        {
            if (line.StartsWith("Saldo="))
            {
                saldo = GetSaldo(line);
            }
            else if (line.StartsWith("CurrencyType="))
            {
                currencyType = GetCurrencyType(line);
            }
            else if (line.StartsWith("RowsToDisplay="))
            {
                rowsToDisplay = GetRowsToDisplay(line);
            }
        }

        Console.WriteLine("Loaded Settings:");
        Console.WriteLine($"Saldo: {saldo}");
        Console.WriteLine($"Currency Type: {currencyType}");
        Console.WriteLine($"Rows to Display: {rowsToDisplay}");
    }

    private static string GetSaldo(string line)
    {
        string saldo;
        saldo = line.Substring("Saldo=".Length).Trim();
        try
        {
            Core.GlobalSaldo = double.Parse(saldo);
        }
        catch (Exception)
        {
            saldo = "0,00";
        }
        return saldo;
    }

    private static string GetCurrencyType(string line)
    {
        string currencyType;
        currencyType = line.Substring("CurrencyType=".Length).Trim();
        Core.GlobalCurrency = Enum.TryParse<Currency>(currencyType, out var field) ? field : Currency.PLN;
        return currencyType;
    }

    private static string GetRowsToDisplay(string line)
    {
        string rowsToDisplay;
        rowsToDisplay = line.Substring("RowsToDisplay=".Length).Trim();
        try
        {
            Core.NumberOfRows = int.Parse(rowsToDisplay);
        }
        catch (Exception)
        {
            Core.NumberOfRows = Constants.NUMBEROFROWS;
        }

        return rowsToDisplay;
    }

    private static bool IsFileExist(string filePath)
    {
        if (File.Exists(filePath)) return true;
        Console.WriteLine("Settings file not found.");
        return false;
    }
}
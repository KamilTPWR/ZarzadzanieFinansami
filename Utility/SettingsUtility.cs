using System.IO;

namespace ZarzadzanieFinansami;

public class SettingsUtility
{
    public static void SaveSettings(string saldoReadyToBeSaved, DataRange dataRange, Currency currencyType, int rowsToDisplay, string filePath = "settings.ini")
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("[Finance]");
            writer.WriteLine($"Saldo={saldoReadyToBeSaved}");
            writer.WriteLine($"DataRange={dataRange.ToString()}");
            writer.WriteLine("[Display]");
            writer.WriteLine($"CurrencyType={currencyType.ToString()}");
            writer.WriteLine($"RowsToDisplay={rowsToDisplay}");
        }
    }

    public static string GetSaldoa(string filePath = "settings.ini")
    {
        if (!IsFileExist(filePath)) return Core.GlobalSaldo.ToString();
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            return line.Substring("Saldo=".Length).Trim();;
        }
        return Core.GlobalSaldo.ToString();
    }
    public static Currency GetCurrencyType(string filePath = "settings.ini")
    {
        if (!IsFileExist(filePath)) return Core.GlobalCurrency;
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string s = line.Substring("CurrencyType=".Length).Trim();
            if (Enum.TryParse(s, out Currency currencyType))
                return currencyType;
        }
        return Core.GlobalCurrency;
    }
    public static DataRange GetDataRange(string filePath = "settings.ini")
    {
        if (!IsFileExist(filePath)) return Core.GlobalDataRange;
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string s = line.Substring("DataRange=".Length).Trim();
            if (Enum.TryParse(s, out DataRange dataRange))
                return dataRange;
        }
        return Core.GlobalDataRange;
    }
    public static int GetRowsToDisplay(string filePath = "settings.ini")
    {
        if (!IsFileExist(filePath)) return Core.NumberOfRows;
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string s = line.Substring("RowsToDisplay=".Length).Trim();
            if (int.TryParse(s, out int rowsToDisplay)) return rowsToDisplay;
        }
        return Core.NumberOfRows;
    }
    
    public static void DebugLoadSettings(string filePath = "settings.ini")
    {
        if (!IsFileExist(filePath)) return;
        string[] lines = File.ReadAllLines(filePath);
        string saldo = string.Empty;
        string currencyType = string.Empty;
        string rowsToDisplay = string.Empty;
        string dataRange = string.Empty;

        foreach (string line in lines)
        {
            if (line.StartsWith("Saldo="))
            {
                saldo = ReadSaldo(line);
            }
            else if (line.StartsWith("DataRange="))
            {
                dataRange = ReadDataRange(line);
            }
            else if (line.StartsWith("CurrencyType="))
            {
                currencyType = ReadCurrencyType(line);
            }
            else if (line.StartsWith("RowsToDisplay="))
            {
                rowsToDisplay = ReadRowsToDisplay(line);
            }
        }

        Console.WriteLine("Loaded Settings:");
        Console.WriteLine($"Saldo: {saldo}");
        Console.WriteLine($"Data Range: {dataRange}");
        Console.WriteLine($"Currency Type: {currencyType}");
        Console.WriteLine($"Rows to Display: {rowsToDisplay}");
    }

    private static string ReadDataRange(string line)
    {
        string s = line.Substring("DataRange=".Length).Trim();
        Core.GlobalDataRange = Enum.TryParse<DataRange>(s, out var rowsToDisplay) ? rowsToDisplay : Core.GlobalDataRange;
        return s;
    }

    private static string ReadSaldo(string line)
    {
        string saldo = line.Substring("Saldo=".Length).Trim();
        Core.GlobalSaldo = double.TryParse(saldo, out var saldoValue) ? saldoValue : 0.00;
        return saldo;
    }

    private static string ReadCurrencyType(string line)
    {
        string currencyType = line.Substring("CurrencyType=".Length).Trim();
        Core.GlobalCurrency = Enum.TryParse<Currency>(currencyType, out var field) ? field : Core.GlobalCurrency;
        return currencyType;
    }

    private static string ReadRowsToDisplay(string line)
    {
        string rowsToDisplay = line.Substring("RowsToDisplay=".Length).Trim();
        Core.NumberOfRows = int.TryParse(rowsToDisplay, out var numRows) ? numRows : Constants.NUMBEROFROWS;
        return rowsToDisplay;
    }

    private static bool IsFileExist(string filePath)
    {
        if (File.Exists(filePath)) return true;
        Console.WriteLine("Settings file not found.");
        return false;
    }
}
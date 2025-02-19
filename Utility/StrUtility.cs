﻿namespace ZarzadzanieFinansami;

public abstract class StrUtility
{
    public static bool IsNumberFormatValid(string imputS)
    {
        const string s = "1234567890,";
        
        foreach(char c in imputS)
        {
            if (!s.Contains(c.ToString()))
                return false;
        }
        return true;
    }
    public static string CropString(string input, int size)
    {
        return input.Length <= size ? input : input.Substring(0, size);
    }
    public static int NumberOfDigitsAfterComa(string liczba)
    {
        int indeksPrzecinka = liczba.IndexOf(',');
        if (indeksPrzecinka == -1) 
            return 0;
        return liczba.Length - indeksPrzecinka - 1;
    }
    public static string FormatValue(double number)
    {
        return Math.Round(number, 2).ToString();
    }
}

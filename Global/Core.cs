namespace ZarzadzanieFinansami;

public static class Core
{
    public static int NumberOfRows = Constants.NUMBEROFROWS;
    public static int Page = 1;

    public static double GlobalSaldo = 0;
    public static Currency GlobalCurrency = Currency.USD;
    public static DataRange GlobalDataRange = DataRange.Year;
    
    public static int PagesNumber()
    {
        return (int)Math.Ceiling((double)DbUtility.GetNumberOfTransactions() / NumberOfRows);
    }
    public static void Navigate(int direction)
    {
        var newPage = Page + direction;
        if (newPage < 1 || newPage > PagesNumber()) return;
        Page = newPage;
    }
}
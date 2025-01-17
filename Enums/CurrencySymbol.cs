namespace ZarzadzanieFinansami;

public static class CurrencySymbol
{
    public static readonly Dictionary<Currency, string> Currency  = new()
    {
        { ZarzadzanieFinansami.Currency.PLN, "zł" },
        { ZarzadzanieFinansami.Currency.USD, "$" },
        { ZarzadzanieFinansami.Currency.EUR, "\u20ac" },
        { ZarzadzanieFinansami.Currency.GPD, "\u00a3"},
        { ZarzadzanieFinansami.Currency.YEN, "\u00a5" }
    };
}
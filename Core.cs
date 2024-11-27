namespace ZarzadzanieFinansami;

public class Core
{
    public static int NumberOfRows = 20;
    public static int Page = 1;
    public double Saldo { get; set; }

    public Core()
    {
        Saldo = 0;
    }

    public void SetSaldo(double newValue)
    {
        Saldo = newValue;
    }

    public static int PagesNumber()
    {
        return (int)Math.Ceiling((double)DbUtility.GetNumberOfTransactions() / NumberOfRows);
    }
}
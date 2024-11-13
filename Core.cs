namespace ZarządzanieFinansami;

public class Core
{
    public double Saldo { get; set; }

    public Core()
    {
        Saldo = 0;
    }
    public void ChangeSaldo(double newValue)
    {
        Saldo = newValue;
    }

}